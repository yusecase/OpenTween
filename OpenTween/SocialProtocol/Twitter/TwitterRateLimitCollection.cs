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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using OpenTween.Api;
using OpenTween.Api.DataModel;

namespace OpenTween.SocialProtocol.Twitter
{
    public class TwitterRateLimitCollection : RateLimitCollection
    {
        public void UpdateFromHeader(HttpResponseHeaders header, string endpointName)
            => this.UpdateFromHeader(header.ToDictionary(x => x.Key, x => string.Join(",", x.Value), StringComparer.OrdinalIgnoreCase), endpointName);

        public void UpdateFromHeader(IDictionary<string, string> header, string endpointName)
        {
            var rateLimit = ParseRateLimit(header, "X-Rate-Limit-");
            if (rateLimit != null)
                this[endpointName] = rateLimit;
        }

        public void UpdateFromJson(TwitterRateLimits json)
        {
            var rateLimits =
                from res in json.Resources
                from item in res.Value
                select (
                    EndpointName: item.Key,
                    Limit: new ApiLimit(
                        item.Value.Limit,
                        item.Value.Remaining,
                        DateTimeUtc.FromUnixTime(item.Value.Reset)
                    )
                );

            this.AddAll(rateLimits.ToDictionary(x => x.EndpointName, x => x.Limit));
        }

        internal static ApiLimit? ParseRateLimit(IDictionary<string, string> header, string prefix)
        {
            var limitCount = (int?)ParseHeaderValue(header, prefix + "Limit");
            var limitRemain = (int?)ParseHeaderValue(header, prefix + "Remaining");
            var limitReset = ParseHeaderValue(header, prefix + "Reset");

            if (limitCount == null || limitRemain == null || limitReset == null)
                return null;

            var limitResetDate = DateTimeUtc.FromUnixTime(limitReset.Value);
            return new ApiLimit(limitCount.Value, limitRemain.Value, limitResetDate);
        }

        internal static long? ParseHeaderValue(IDictionary<string, string> dict, params string[] keys)
        {
            foreach (var key in keys)
            {
                if (!dict.ContainsKey(key)) continue;

                if (long.TryParse(dict[key], out var result))
                    return result;
            }

            return null;
        }
    }
}
