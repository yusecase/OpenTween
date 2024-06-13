// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Api.Misskey;
using OpenTween.SocialProtocol.Misskey;

namespace OpenTween.Connection
{
    public sealed class MisskeyApiConnection : IApiConnection, IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        internal HttpClient Http;

        private readonly Uri apiBaseUri;
        private readonly string accessToken;
        private readonly MisskeyAccountState accountState;

        public MisskeyApiConnection(Uri apiBaseUri, string accessToken, MisskeyAccountState accountState)
        {
            this.apiBaseUri = apiBaseUri;
            this.accessToken = accessToken;
            this.accountState = accountState;

            this.InitializeHttpClients();
            Networking.WebProxyChanged += this.Networking_WebProxyChanged;
        }

        [MemberNotNull(nameof(Http))]
        private void InitializeHttpClients()
        {
            this.Http = InitializeHttpClient();

            // タイムアウト設定は IHttpRequest.Timeout でリクエスト毎に制御する
            this.Http.Timeout = Timeout.InfiniteTimeSpan;
        }

        public void ThrowIfUnauthorizedScope(string scope)
        {
            if (!this.accountState.AuthorizedScopes.Contains(scope))
                throw new AdditionalScopeRequiredException();
        }

        public async Task<ApiResponse> SendAsync(IHttpRequest request)
        {
            using var requestMessage = request.CreateMessage(this.apiBaseUri);

            if (!MyCommon.IsNullOrEmpty(this.accessToken))
                requestMessage.Headers.Authorization = new("Bearer", this.accessToken);

            HttpResponseMessage? responseMessage = null;
            try
            {
                responseMessage = await TwitterApiConnection.HandleTimeout(
                    (token) => this.Http.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, token),
                    request.Timeout
                );

                await CheckStatusCode(responseMessage)
                    .ConfigureAwait(false);

                var response = new ApiResponse(responseMessage);
                responseMessage = null; // responseMessage は ApiResponse で使用するため破棄されないようにする

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw MisskeyApiException.CreateFromException(ex);
            }
            catch (OperationCanceledException ex)
            {
                throw MisskeyApiException.CreateFromException(ex);
            }
            finally
            {
                responseMessage?.Dispose();
            }
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            Networking.WebProxyChanged -= this.Networking_WebProxyChanged;
            this.Http.Dispose();

            this.IsDisposed = true;
        }

        private void Networking_WebProxyChanged(object sender, EventArgs e)
            => this.InitializeHttpClients();

        private static async Task CheckStatusCode(HttpResponseMessage response)
        {
            var statusCode = response.StatusCode;

            if ((int)statusCode >= 200 && (int)statusCode <= 299)
                return;

            string responseText;
            using (var content = response.Content)
            {
                responseText = await content.ReadAsStringAsync()
                    .ConfigureAwait(false);
            }

            if (string.IsNullOrWhiteSpace(responseText))
                throw new MisskeyApiException(statusCode, responseText);

            try
            {
                var error = MisskeyError.ParseJson(responseText);

                throw new MisskeyApiException(statusCode, error, responseText);
            }
            catch (SerializationException)
            {
                throw new MisskeyApiException(statusCode, responseText);
            }
        }

        private static HttpClient InitializeHttpClient()
        {
            var builder = Networking.CreateHttpClientBuilder();

            builder.SetupHttpClientHandler(
                x => x.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache)
            );

            return builder.Build();
        }
    }
}
