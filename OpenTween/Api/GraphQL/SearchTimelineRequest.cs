// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using OpenTween.Connection;

namespace OpenTween.Api.GraphQL
{
    public class SearchTimelineRequest
    {
        public static readonly string EndpointName = "SearchTimeline";

        private const string DefaultQueryId = "GcXk9vN_d1jUfHNqLacXQA";

        public string RawQuery { get; set; }

        public int Count { get; set; } = 20;

        public TwitterGraphqlCursor? Cursor { get; set; }

        public SearchTimelineRequest(string rawQuery)
            => this.RawQuery = rawQuery;

        private static Uri EndpointUri
        {
            get
            {
                var queryId = Environment.GetEnvironmentVariable("OPENTWEEN_TWITTER_QID_SEARCH_TIMELINE");
                if (MyCommon.IsNullOrEmpty(queryId))
                    queryId = DefaultQueryId;

                return new($"https://twitter.com/i/api/graphql/{Uri.EscapeDataString(queryId)}/SearchTimeline");
            }
        }

        public Dictionary<string, string> CreateParameters()
        {
            var cursorStr = this.Cursor?.Value;

            return new()
            {
                ["variables"] = "{" +
                    $@"""rawQuery"":""{JsonUtils.EscapeJsonString(this.RawQuery)}""," +
                    $@"""count"":{this.Count}," +
                    @"""querySource"":""typed_query""," +
                    $@"""product"":""Latest""" +
                    (cursorStr != null ? $@",""cursor"":""{JsonUtils.EscapeJsonString(cursorStr)}""" : "") +
                    "}",
                ["features"] = "{" +
                    @"""responsive_web_graphql_exclude_directive_enabled"":true," +
                    @"""verified_phone_label_enabled"":false," +
                    @"""responsive_web_graphql_timeline_navigation_enabled"":true," +
                    @"""responsive_web_graphql_skip_user_profile_image_extensions_enabled"":false," +
                    @"""creator_subscriptions_tweet_preview_api_enabled"":true," +
                    @"""tweetypie_unmention_optimization_enabled"":true," +
                    @"""responsive_web_edit_tweet_api_enabled"":true," +
                    @"""graphql_is_translatable_rweb_tweet_is_translatable_enabled"":true," +
                    @"""view_counts_everywhere_api_enabled"":true," +
                    @"""longform_notetweets_consumption_enabled"":true," +
                    @"""responsive_web_twitter_article_tweet_consumption_enabled"":true," +
                    @"""tweet_awards_web_tipping_enabled"":false," +
                    @"""freedom_of_speech_not_reach_fetch_enabled"":true," +
                    @"""standardized_nudges_misinfo"":true," +
                    @"""tweet_with_visibility_results_prefer_gql_limited_actions_policy_enabled"":true," +
                    @"""longform_notetweets_rich_text_read_enabled"":true," +
                    @"""longform_notetweets_inline_media_enabled"":true," +
                    @"""responsive_web_enhance_cards_enabled"":false," +
                    @"""articles_preview_enabled"":true," +
                    @"""creator_subscriptions_quote_tweet_preview_enabled"":false," +
                    @"""c9s_tweet_anatomy_moderator_badge_enabled"":true," +
                    @"""responsive_web_twitter_article_notes_tab_enabled"":true," +
                    @"""rweb_tipjar_consumption_enabled"":true," +
                    @"""communities_web_enable_tweet_community_results_fetch"":true," +
                    @"""rweb_video_timestamps_enabled"":true" +
                    "}",
                ["fieldToggles"] = "{" +
                    @"""withArticleRichContentState"":false" +
                    "}",
            };
        }

        public async Task<TimelineGraphqlResponse> Send(IApiConnection apiConnection)
        {
            var request = new GetRequest
            {
                RequestUri = EndpointUri,
                Query = this.CreateParameters(),
                EndpointName = EndpointName,
            };

            using var response = await apiConnection.SendAsync(request)
                .ConfigureAwait(false);

            var rootElm = await response.ReadAsJsonXml()
                .ConfigureAwait(false);

            return TimelineResponseParser.Parse(rootElm);
        }
    }
}
