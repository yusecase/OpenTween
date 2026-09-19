// OpenTween - Client of Twitter
// Copyright (c) 2026 OpenTween contributors
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml.XPath;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using OpenTween.Api.GraphQL;

namespace OpenTween.SocialProtocol.Twitter
{
    internal class SearchTimelineWebView2Fetcher
    {
        private static readonly TimeSpan FetchTimeout = TimeSpan.FromSeconds(45);
        private const int ScrollIntervalMilliseconds = 1200;
        private const int MaxScrollAttempts = 20;

        public Task<TimelineGraphqlResponse> FetchAsync(string rawCookie, string rawQuery, int count)
        {
            var tcs = new TaskCompletionSource<TimelineGraphqlResponse>();
            var thread = new Thread(() => this.RunThread(rawCookie, rawQuery, count, tcs))
            {
                IsBackground = true,
                Name = "OpenTween SearchTimeline WebView2",
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return tcs.Task;
        }

        private void RunThread(string rawCookie, string rawQuery, int count, TaskCompletionSource<TimelineGraphqlResponse> tcs)
        {
            using var form = new FetchForm(rawCookie, rawQuery, count, tcs);
            using var timer = new System.Windows.Forms.Timer { Interval = (int)FetchTimeout.TotalMilliseconds };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                if (!form.CompleteIfAnyResult())
                    tcs.TrySetException(new WebApiException("Timeout"));

                form.Close();
            };
            timer.Start();

            Application.Run(form);
        }

        private sealed class FetchForm : Form
        {
            private readonly string rawCookie;
            private readonly string rawQuery;
            private readonly int count;
            private readonly TaskCompletionSource<TimelineGraphqlResponse> tcs;
            private readonly WebView2 webView = new() { Dock = DockStyle.Fill };
            private readonly JavaScriptSerializer json = new();
            private readonly HashSet<string> pendingSearchRequests = new();
            private readonly List<TimelineTweet> tweets = new();
            private readonly HashSet<string> tweetIds = new(StringComparer.Ordinal);
            private readonly System.Windows.Forms.Timer scrollTimer;
            private int scrollAttempts;

            public FetchForm(string rawCookie, string rawQuery, int count, TaskCompletionSource<TimelineGraphqlResponse> tcs)
            {
                this.rawCookie = rawCookie;
                this.rawQuery = rawQuery;
                this.count = Math.Max(count, 1);
                this.tcs = tcs;
                this.scrollTimer = new() { Interval = ScrollIntervalMilliseconds };
                this.scrollTimer.Tick += async (_, _) => await this.ScrollTimerTickAsync();

                this.ShowInTaskbar = false;
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(-32000, -32000);
                this.Width = 640;
                this.Height = 480;
                this.Controls.Add(this.webView);

                this.Shown += async (_, _) => await this.InitializeAsync();
                this.tcs.Task.ContinueWith(_ => this.BeginInvoke(new Action(this.Close)));
            }

            public bool CompleteIfAnyResult()
            {
                if (this.tweets.Count == 0)
                    return false;

                return this.tcs.TrySetResult(this.CreateResponse());
            }

            private async Task InitializeAsync()
            {
                try
                {
                    var userDataFolder = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "OpenTweenSearchTimelineWebView2");
                    var environment = await CoreWebView2Environment.CreateAsync(browserExecutableFolder: null, userDataFolder)
                        .ConfigureAwait(true);

                    await this.webView.EnsureCoreWebView2Async(environment)
                        .ConfigureAwait(true);

                    var core = this.webView.CoreWebView2;
                    await core.CallDevToolsProtocolMethodAsync("Network.enable", "{}")
                        .ConfigureAwait(true);
                    await core.CallDevToolsProtocolMethodAsync("Network.setCacheDisabled", "{\"cacheDisabled\":true}")
                        .ConfigureAwait(true);

                    core.GetDevToolsProtocolEventReceiver("Network.responseReceived")
                        .DevToolsProtocolEventReceived += this.NetworkResponseReceived;
                    core.GetDevToolsProtocolEventReceiver("Network.loadingFinished")
                        .DevToolsProtocolEventReceived += this.NetworkLoadingFinished;

                    this.SetCookies();
                    this.NavigateSearch();
                }
                catch (Exception ex)
                {
                    this.tcs.TrySetException(new WebApiException($"Failed to initialize WebView2 search: {ex.Message}", ex));
                }
            }

            private void SetCookies()
            {
                foreach (var pair in ParseCookiePairs(this.rawCookie))
                {
                    this.AddCookie(pair.Key, pair.Value, ".x.com");
                    this.AddCookie(pair.Key, pair.Value, ".twitter.com");
                }
            }

            private void AddCookie(string name, string value, string domain)
            {
                var cookie = this.webView.CoreWebView2.CookieManager.CreateCookie(name, value, domain, "/");
                cookie.IsHttpOnly = true;
                cookie.IsSecure = true;
                this.webView.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);
            }

            private void NavigateSearch()
            {
                var query = Uri.EscapeDataString(this.rawQuery);
                this.webView.CoreWebView2.Navigate($"https://x.com/search?q={query}&f=live");
            }

            private void NetworkResponseReceived(object? sender, CoreWebView2DevToolsProtocolEventReceivedEventArgs e)
            {
                try
                {
                    var data = this.json.Deserialize<Dictionary<string, object>>(e.ParameterObjectAsJson);
                    var requestId = data.TryGetValue("requestId", out var requestIdObj) ? requestIdObj?.ToString() : null;
                    var response = data.TryGetValue("response", out var responseObj) ? responseObj as Dictionary<string, object> : null;
                    var url = response != null && response.TryGetValue("url", out var urlObj) ? urlObj?.ToString() : "";
                    var status = response != null && response.TryGetValue("status", out var statusObj) ? Convert.ToInt32(statusObj) : 0;

                    if (requestId == null || url == null || !url.Contains("/SearchTimeline"))
                        return;

                    if (status == 200)
                        this.pendingSearchRequests.Add(requestId);
                }
                catch (Exception ex)
                {
                    this.tcs.TrySetException(new WebApiException("Failed to inspect SearchTimeline response", ex));
                }
            }

            private async void NetworkLoadingFinished(object? sender, CoreWebView2DevToolsProtocolEventReceivedEventArgs e)
            {
                string? requestId = null;
                try
                {
                    var data = this.json.Deserialize<Dictionary<string, object>>(e.ParameterObjectAsJson);
                    requestId = data.TryGetValue("requestId", out var requestIdObj) ? requestIdObj?.ToString() : null;
                    if (requestId == null || !this.pendingSearchRequests.Remove(requestId))
                        return;

                    string response;
                    try
                    {
                        response = await this.webView.CoreWebView2.CallDevToolsProtocolMethodAsync(
                            "Network.getResponseBody",
                            "{\"requestId\":\"" + EscapeJson(requestId) + "\"}")
                            .ConfigureAwait(true);
                    }
                    catch (Exception ex)
                    {
                        MyCommon.TraceOut($"Failed to read SearchTimeline response body: requestId={requestId}, message={ex.Message}");
                        return;
                    }

                    var bodyEnvelope = this.json.Deserialize<Dictionary<string, object>>(response);
                    var body = bodyEnvelope.TryGetValue("body", out var bodyObj) ? bodyObj?.ToString() ?? "" : "";
                    var base64 = bodyEnvelope.TryGetValue("base64Encoded", out var base64Obj) && Convert.ToBoolean(base64Obj);
                    if (base64)
                        body = Encoding.UTF8.GetString(Convert.FromBase64String(body));

                    var responseGraphql = TimelineResponseParser.ParseJson(body);
                    this.AddTweets(responseGraphql);

                    if (this.tweets.Count >= this.count)
                    {
                        this.tcs.TrySetResult(this.CreateResponse());
                        return;
                    }

                    this.ScheduleNextScroll();
                }
                catch (Exception ex)
                {
                    this.tcs.TrySetException(new WebApiException("Failed to read SearchTimeline response body", ex));
                }
            }

            private void AddTweets(TimelineGraphqlResponse response)
            {
                foreach (var tweet in response.Tweets)
                {
                    var id = tweet.Element.XPathSelectElement("tweet_results/result/rest_id|tweet_results/result/tweet/rest_id")?.Value;
                    if (id == null || !this.tweetIds.Add(id))
                        continue;

                    this.tweets.Add(tweet);
                    if (this.tweets.Count >= this.count)
                        break;
                }
            }

            private TimelineGraphqlResponse CreateResponse()
                => new(this.tweets.Take(this.count).ToArray(), CursorTop: null, CursorBottom: null);

            private void ScheduleNextScroll()
            {
                if (this.scrollAttempts >= MaxScrollAttempts)
                {
                    this.CompleteIfAnyResult();
                    return;
                }

                this.scrollTimer.Stop();
                this.scrollTimer.Start();
            }

            private async Task ScrollTimerTickAsync()
            {
                this.scrollTimer.Stop();

                if (this.tcs.Task.IsCompleted)
                    return;

                if (this.scrollAttempts >= MaxScrollAttempts)
                {
                    this.CompleteIfAnyResult();
                    return;
                }

                this.scrollAttempts++;
                await this.webView.CoreWebView2.ExecuteScriptAsync("window.scrollTo(0, document.body.scrollHeight);")
                    .ConfigureAwait(true);

                this.ScheduleNextScroll();
            }

            private static Dictionary<string, string> ParseCookiePairs(string rawCookie)
            {
                var pairs = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var part in rawCookie.Split(';'))
                {
                    var trimmed = part.Trim();
                    if (trimmed.Length == 0)
                        continue;

                    var index = trimmed.IndexOf('=');
                    if (index <= 0)
                        continue;

                    var name = trimmed.Substring(0, index);
                    if (name != "auth_token" && name != "ct0")
                        continue;

                    pairs[name] = trimmed.Substring(index + 1);
                }

                return pairs;
            }

            private static string EscapeJson(string value)
                => value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
