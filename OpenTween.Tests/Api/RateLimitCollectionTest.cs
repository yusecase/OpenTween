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

using Xunit;

namespace OpenTween.Api
{
    public class RateLimitCollectionTest
    {
        [Fact]
        public void SubscribeAccessLimitUpdated_Test()
        {
            var collection = new RateLimitCollection();
            var count = 0;

            void Handler(RateLimitCollection sender, RateLimitCollection.AccessLimitUpdatedEventArgs e)
            {
                Assert.Same(collection, sender);
                Assert.Equal("/statuses/home_timeline", e.EndpointName);
                count++;
            }

            using (collection.SubscribeAccessLimitUpdated(Handler))
            {
                collection["/statuses/home_timeline"] = new(150, 100, new(2013, 1, 1, 0, 0, 0));
                Assert.Equal(1, count);
            }

            collection["/statuses/home_timeline"] = new(150, 99, new(2013, 1, 1, 0, 0, 0));
            Assert.Equal(1, count); // 変化しない
        }

        [Fact]
        public void SubscribeAccessLimitUpdated_ClearTest()
        {
            var collection = new RateLimitCollection();
            collection["/statuses/home_timeline"] = new(150, 100, new(2013, 1, 1, 0, 0, 0));

            var count = 0;

            void Handler(RateLimitCollection sender, RateLimitCollection.AccessLimitUpdatedEventArgs e)
            {
                Assert.Same(collection, sender);
                Assert.Null(e.EndpointName);
                count++;
            }

            using (collection.SubscribeAccessLimitUpdated(Handler))
            {
                collection.Clear();
                Assert.Equal(1, count);
            }
        }
    }
}
