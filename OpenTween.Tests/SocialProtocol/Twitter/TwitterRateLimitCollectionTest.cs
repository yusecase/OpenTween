// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System;
using System.Collections.Generic;
using System.Net.Http;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using Xunit;

namespace OpenTween.SocialProtocol.Twitter
{
    public class TwitterRateLimitCollectionTest
    {
        [Fact]
        public void Clear_Test()
        {
            var collection = new TwitterRateLimitCollection();

            collection["/statuses/home_timeline"] = new(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0));
            collection.Clear();

            Assert.Null(collection["/statuses/home_timeline"]);
        }

        public static readonly TheoryData<Dictionary<string, string>, ApiLimit?> ParseRateLimitTestCase = new()
        {
            {
                new Dictionary<string, string>
                {
                    ["X-RateLimit-Limit"] = "150",
                    ["X-RateLimit-Remaining"] = "100",
                    ["X-RateLimit-Reset"] = "1356998400",
                },
                new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0))
            },
            {
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["x-ratelimit-limit"] = "150",
                    ["x-ratelimit-remaining"] = "100",
                    ["x-ratelimit-reset"] = "1356998400",
                },
                new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0))
            },
            {
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["X-RateLimit-Limit"] = "150",
                    ["X-RateLimit-Remaining"] = "100",
                    ["X-RateLimit-Reset"] = "hogehoge",
                },
                null
            },
            {
                new Dictionary<string, string>
                {
                    ["X-RateLimit-Limit"] = "150",
                    ["X-RateLimit-Remaining"] = "100",
                },
                null
            },
        };

        [Theory]
        [MemberData(nameof(ParseRateLimitTestCase))]
        public void ParseRateLimitTest(IDictionary<string, string> header, ApiLimit? expected)
        {
            var limit = TwitterRateLimitCollection.ParseRateLimit(header, "X-RateLimit-");
            Assert.Equal(expected, limit);
        }

        [Fact]
        public void UpdateFromHeader_DictionaryTest()
        {
            var collection = new TwitterRateLimitCollection();

            var header = new Dictionary<string, string>
            {
                ["X-Rate-Limit-Limit"] = "150",
                ["X-Rate-Limit-Remaining"] = "100",
                ["X-Rate-Limit-Reset"] = "1356998400",
            };
            collection.UpdateFromHeader(header, "/statuses/home_timeline");

            var rateLimit = collection["/statuses/home_timeline"]!;
            Assert.Equal(150, rateLimit.AccessLimitCount);
            Assert.Equal(100, rateLimit.AccessLimitRemain);
            Assert.Equal(new DateTimeUtc(2013, 1, 1, 0, 0, 0), rateLimit.AccessLimitResetDate);
        }

        [Fact]
        public void UpdateFromHeader_HttpClientTest()
        {
            var collection = new TwitterRateLimitCollection();

            var response = new HttpResponseMessage
            {
                Headers =
                {
                    { "x-rate-limit-limit", "150" },
                    { "x-rate-limit-remaining", "100" },
                    { "x-rate-limit-reset", "1356998400" },
                },
            };
            collection.UpdateFromHeader(response.Headers, "/statuses/home_timeline");

            var rateLimit = collection["/statuses/home_timeline"]!;
            Assert.Equal(150, rateLimit.AccessLimitCount);
            Assert.Equal(100, rateLimit.AccessLimitRemain);
            Assert.Equal(new DateTimeUtc(2013, 1, 1, 0, 0, 0), rateLimit.AccessLimitResetDate);
        }

        [Fact]
        public void UpdateFromJsonTest()
        {
            var collection = new TwitterRateLimitCollection();

            var json = """{"resources":{"statuses":{"/statuses/home_timeline":{"limit":150,"remaining":100,"reset":1356998400}}}}""";
            collection.UpdateFromJson(TwitterRateLimits.ParseJson(json));

            var rateLimit = collection["/statuses/home_timeline"]!;
            Assert.Equal(150, rateLimit.AccessLimitCount);
            Assert.Equal(100, rateLimit.AccessLimitRemain);
            Assert.Equal(new DateTimeUtc(2013, 1, 1, 0, 0, 0), rateLimit.AccessLimitResetDate);
        }
    }
}
