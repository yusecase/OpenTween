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
using OpenTween.Models;
using Xunit;

namespace OpenTween.SocialProtocol.Twitter
{
    public class RelatedTweetsFetcherTest
    {
        [Fact]
        public void FindTopOfReplyChainTest()
        {
            var posts = new Dictionary<PostId, PostClass>
            {
                [new TwitterStatusId("950")] = new PostClass { StatusId = new TwitterStatusId("950"), InReplyToStatusId = null }, // このツイートが末端
                [new TwitterStatusId("987")] = new PostClass { StatusId = new TwitterStatusId("987"), InReplyToStatusId = new TwitterStatusId("950") },
                [new TwitterStatusId("999")] = new PostClass { StatusId = new TwitterStatusId("999"), InReplyToStatusId = new TwitterStatusId("987") },
                [new TwitterStatusId("1000")] = new PostClass { StatusId = new TwitterStatusId("1000"), InReplyToStatusId = new TwitterStatusId("999") },
            };
            Assert.Equal(new TwitterStatusId("950"), RelatedTweetsFetcher.FindTopOfReplyChain(posts, new TwitterStatusId("1000")).StatusId);
            Assert.Equal(new TwitterStatusId("950"), RelatedTweetsFetcher.FindTopOfReplyChain(posts, new TwitterStatusId("950")).StatusId);
            Assert.Throws<ArgumentException>(() => RelatedTweetsFetcher.FindTopOfReplyChain(posts, new TwitterStatusId("500")));

            posts = new Dictionary<PostId, PostClass>
            {
                // new TwitterStatusId("1200") は posts の中に存在しない
                [new TwitterStatusId("1210")] = new PostClass { StatusId = new TwitterStatusId("1210"), InReplyToStatusId = new TwitterStatusId("1200") },
                [new TwitterStatusId("1220")] = new PostClass { StatusId = new TwitterStatusId("1220"), InReplyToStatusId = new TwitterStatusId("1210") },
                [new TwitterStatusId("1230")] = new PostClass { StatusId = new TwitterStatusId("1230"), InReplyToStatusId = new TwitterStatusId("1220") },
            };
            Assert.Equal(new TwitterStatusId("1210"), RelatedTweetsFetcher.FindTopOfReplyChain(posts, new TwitterStatusId("1230")).StatusId);
            Assert.Equal(new TwitterStatusId("1210"), RelatedTweetsFetcher.FindTopOfReplyChain(posts, new TwitterStatusId("1210")).StatusId);
        }
    }
}
