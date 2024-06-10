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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.XPath;
using OpenTween.Connection;

namespace OpenTween.Api.GraphQL
{
    public class ViewerRequest
    {
        public static readonly string EndpointName = "Viewer";

        private static readonly Uri EndpointUri = new("https://api.twitter.com/graphql/-876iyxD1O_0X0BqeykjZA/Viewer");

        public Dictionary<string, string> CreateParameters()
        {
            return new()
            {
                ["variables"] = """
                    {"withCommunitiesMemberships":true}
                    """,
                ["features"] = """
                    {"rweb_tipjar_consumption_enabled":true,"responsive_web_graphql_exclude_directive_enabled":true,"verified_phone_label_enabled":false,"creator_subscriptions_tweet_preview_api_enabled":true,"responsive_web_graphql_skip_user_profile_image_extensions_enabled":false,"responsive_web_graphql_timeline_navigation_enabled":true}
                    """,
                ["fieldToggles"] = """
                    {"isDelegate":false,"withAuxiliaryUserLabels":false}
                    """,
            };
        }

        public async Task<TwitterGraphqlUser> Send(IApiConnection apiConnection)
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

            ErrorResponse.ThrowIfError(rootElm);

            try
            {
                var userElm = rootElm.XPathSelectElement("/data/viewer/user_results/result")
                    ?? throw new WebApiException("Parse error");

                return new(userElm);
            }
            catch (WebApiException ex)
            {
                ex.ResponseText = JsonUtils.JsonXmlToString(rootElm);
                throw;
            }
        }
    }
}
