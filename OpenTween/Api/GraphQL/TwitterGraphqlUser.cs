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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api.DataModel;

namespace OpenTween.Api.GraphQL
{
    public class TwitterGraphqlUser
    {
        public const string TypeName = "User";

        public XElement Element { get; }

        public TwitterGraphqlUser(XElement element)
        {
            var typeName = element.Element("__typename")?.Value;
            if (typeName != TypeName)
                throw new ArgumentException($"Invalid itemType: {typeName}", nameof(element));

            this.Element = element;
        }

        public TwitterUser ToTwitterUser()
        {
            try
            {
                return TwitterGraphqlUser.ParseUser(this.Element);
            }
            catch (WebApiException ex)
            {
                ex.ResponseText = JsonUtils.JsonXmlToString(this.Element);
                MyCommon.TraceOut(ex);
                throw;
            }
        }

        public static TwitterUser ParseUser(XElement userElm)
        {
            var userLegacyElm = userElm.Element("legacy");
            if (userLegacyElm == null)
                return ParseModernUser(userElm);

            static string GetText(XElement elm, string name)
                => elm.Element(name)?.Value ?? throw CreateParseError();

            static string? GetTextOrNull(XElement elm, string name)
                => elm.Element(name)?.Value;

            return new()
            {
                IdStr = GetText(userElm, "rest_id"),
                Name = GetText(userLegacyElm, "name"),
                ProfileImageUrlHttps = GetTextOrNull(userLegacyElm, "profile_image_url_https"),
                ScreenName = GetText(userLegacyElm, "screen_name"),
                Protected = GetTextOrNull(userLegacyElm, "protected") == "true",
                Verified = GetTextOrNull(userLegacyElm, "verified") == "true",
                CreatedAt = GetText(userLegacyElm, "created_at"),
                FollowersCount = int.Parse(GetText(userLegacyElm, "followers_count")),
                FriendsCount = int.Parse(GetText(userLegacyElm, "friends_count")),
                FavouritesCount = int.Parse(GetText(userLegacyElm, "favourites_count")),
                StatusesCount = int.Parse(GetText(userLegacyElm, "statuses_count")),
                Description = GetTextOrNull(userLegacyElm, "description"),
                Location = GetTextOrNull(userLegacyElm, "location"),
                Url = GetTextOrNull(userLegacyElm, "url"),
                Entities = new()
                {
                    Description = new()
                    {
                        Urls = userLegacyElm.XPathSelectElements("entities/description/urls/item")
                            .Select(x => new TwitterEntityUrl()
                            {
                                Indices = x.XPathSelectElements("indices/item").Select(x => int.Parse(x.Value)).ToArray(),
                                DisplayUrl = GetTextOrNull(x, "display_url"),
                                ExpandedUrl = GetTextOrNull(x, "expanded_url"),
                                Url = GetText(x, "url"),
                            })
                            .ToArray(),
                    },
                    Url = new()
                    {
                        Urls = userLegacyElm.XPathSelectElements("entities/url/urls/item")
                            .Select(x => new TwitterEntityUrl()
                            {
                                Indices = x.XPathSelectElements("indices/item").Select(x => int.Parse(x.Value)).ToArray(),
                                DisplayUrl = GetTextOrNull(x, "display_url"),
                                ExpandedUrl = GetTextOrNull(x, "expanded_url"),
                                Url = GetText(x, "url"),
                            })
                            .ToArray(),
                    },
                },
            };
        }

        private static TwitterUser ParseModernUser(XElement userElm)
        {
            var coreElm = userElm.Element("core") ?? throw CreateParseError();
            var profileBioElm = userElm.Element("profile_bio");
            var relationshipCountsElm = userElm.Element("relationship_counts");
            var tweetCountsElm = userElm.Element("tweet_counts");
            var actionCountsElm = userElm.Element("action_counts");

            static string GetText(XElement elm, string name)
                => elm.Element(name)?.Value ?? throw CreateParseError();

            static string? GetTextOrNull(XElement? elm, string name)
                => elm?.Element(name)?.Value;

            static int GetIntOrZero(XElement? elm, string name)
                => int.TryParse(GetTextOrNull(elm, name), out var value) ? value : 0;

            static bool GetBoolOrFalse(XElement? elm, string name)
                => GetTextOrNull(elm, name) == "true";

            return new()
            {
                IdStr = GetText(userElm, "rest_id"),
                Name = GetText(coreElm, "name"),
                ProfileImageUrlHttps = GetTextOrNull(userElm.Element("avatar"), "image_url"),
                ScreenName = GetText(coreElm, "screen_name"),
                Protected = GetBoolOrFalse(userElm.Element("privacy"), "protected"),
                Verified = GetBoolOrFalse(userElm.Element("verification"), "verified"),
                CreatedAt = GetText(coreElm, "created_at"),
                FollowersCount = GetIntOrZero(relationshipCountsElm, "followers"),
                FriendsCount = GetIntOrZero(relationshipCountsElm, "following"),
                FavouritesCount = GetIntOrZero(actionCountsElm, "favorites_count"),
                StatusesCount = GetIntOrZero(tweetCountsElm, "tweets"),
                Description = GetTextOrNull(profileBioElm, "description"),
                Location = GetTextOrNull(userElm.Element("location"), "location"),
                Url = GetTextOrNull(userElm.Element("website"), "url"),
                ProfileBannerUrl = GetTextOrNull(userElm.Element("banner"), "image_url") ?? "",
                Entities = new()
                {
                    Description = new(),
                    Url = new(),
                },
            };
        }

        private static Exception CreateParseError()
            => throw new WebApiException("Parse error on User");
    }
}
