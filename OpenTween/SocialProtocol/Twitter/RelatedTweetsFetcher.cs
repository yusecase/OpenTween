// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2013      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenTween.Models;

namespace OpenTween.SocialProtocol.Twitter
{
    public class RelatedTweetsFetcher
    {
        private readonly TabInformations tabinfo;
        private readonly ISocialProtocolClient client;

        public RelatedTweetsFetcher(TabInformations tabinfo, ISocialProtocolClient client)
        {
            this.tabinfo = tabinfo;
            this.client = client;
        }

        public async Task<PostClass[]> Run(PostClass targetPost, bool firstLoad)
        {
            if (targetPost.RetweetedId != null)
            {
                var originalPost = targetPost with
                {
                    StatusId = targetPost.RetweetedId,
                    RetweetedId = null,
                    RetweetedBy = null,
                };
                targetPost = originalPost;
            }

            var relPosts = new Dictionary<PostId, PostClass>();
            if (targetPost.TextFromApi.Contains("@") && targetPost.InReplyToStatusId == null)
            {
                // 検索結果対応
                var p = this.tabinfo[targetPost.StatusId];
                if (p != null && p.InReplyToStatusId != null)
                {
                    targetPost = p;
                }
                else
                {
                    p = await this.client.GetPostById(targetPost.StatusId, firstLoad)
                        .ConfigureAwait(false);
                    targetPost = p;
                }
            }
            relPosts.Add(targetPost.StatusId, targetPost);

            Exception? lastException = null;

            // in_reply_to_status_id を使用してリプライチェインを辿る
            var nextPost = FindTopOfReplyChain(relPosts, targetPost.StatusId);
            var loopCount = 1;
            while (nextPost.InReplyToStatusId != null && loopCount++ <= 20)
            {
                var inReplyToId = nextPost.InReplyToStatusId;

                var inReplyToPost = this.tabinfo[inReplyToId];
                if (inReplyToPost == null)
                {
                    try
                    {
                        inReplyToPost = await this.client.GetPostById(inReplyToId, firstLoad)
                            .ConfigureAwait(false);
                    }
                    catch (WebApiException ex)
                    {
                        lastException = ex;
                        break;
                    }
                }

                relPosts.Add(inReplyToPost.StatusId, inReplyToPost);

                nextPost = FindTopOfReplyChain(relPosts, nextPost.StatusId);
            }

            // MRTとかに対応のためツイート内にあるツイートを指すURLを取り込む
            var text = targetPost.Text;
            var ma = OpenTween.Twitter.StatusUrlRegex.Matches(text).Cast<Match>()
                .Concat(OpenTween.Twitter.ThirdPartyStatusUrlRegex.Matches(text).Cast<Match>());
            foreach (var match in ma)
            {
                var statusId = new TwitterStatusId(match.Groups["StatusId"].Value);
                if (!relPosts.ContainsKey(statusId))
                {
                    var p = this.tabinfo[statusId];
                    if (p == null)
                    {
                        try
                        {
                            p = await this.client.GetPostById(statusId, firstLoad)
                                .ConfigureAwait(false);
                        }
                        catch (WebApiException ex)
                        {
                            lastException = ex;
                            break;
                        }
                    }

                    if (p != null)
                        relPosts.Add(p.StatusId, p);
                }
            }

            try
            {
                var firstPost = nextPost;
                var posts = await this.GetConversationPosts(firstPost, targetPost)
                    .ConfigureAwait(false);

                foreach (var post in posts.OrderBy(x => x.StatusId))
                {
                    if (relPosts.ContainsKey(post.StatusId))
                        continue;

                    // リプライチェーンが繋がらないツイートは除外
                    if (post.InReplyToStatusId == null || !relPosts.ContainsKey(post.InReplyToStatusId))
                        continue;

                    relPosts.Add(post.StatusId, post);
                }
            }
            catch (WebException ex)
            {
                lastException = ex;
            }

            if (lastException != null)
                throw new WebApiException(lastException.Message, lastException);

            var clonedPosts = relPosts.Values.Select(x => x with { }).ToArray();

            return clonedPosts;
        }

        private async Task<PostClass[]> GetConversationPosts(PostClass firstPost, PostClass targetPost)
        {
            var conversationId = firstPost.StatusId;
            var query = $"conversation_id:{conversationId.Id}";

            if (targetPost.InReplyToUser != null && targetPost.InReplyToUser != targetPost.ScreenName)
                query += $" (from:{targetPost.ScreenName} to:{targetPost.InReplyToUser}) OR (from:{targetPost.InReplyToUser} to:{targetPost.ScreenName})";
            else
                query += $" from:{targetPost.ScreenName} to:{targetPost.ScreenName}";

            var response = await this.client.GetSearchTimeline(query, lang: "", count: 100, cursor: null, firstLoad: false);

            return response.Posts;
        }

        /// <summary>
        /// startStatusId からリプライ先の発言を辿る。発言は posts 以外からは検索しない。
        /// </summary>
        /// <returns>posts の中から検索されたリプライチェインの末端</returns>
        internal static PostClass FindTopOfReplyChain(IDictionary<PostId, PostClass> posts, PostId startStatusId)
        {
            if (!posts.ContainsKey(startStatusId))
                throw new ArgumentException("startStatusId (" + startStatusId.Id + ") が posts の中から見つかりませんでした。", nameof(startStatusId));

            var nextPost = posts[startStatusId];
            while (nextPost.InReplyToStatusId != null)
            {
                if (!posts.ContainsKey(nextPost.InReplyToStatusId))
                    break;
                nextPost = posts[nextPost.InReplyToStatusId];
            }

            return nextPost;
        }
    }
}
