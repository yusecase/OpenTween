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

using System.Collections.Generic;
using OpenTween.Models;
using Xunit;

namespace OpenTween.SocialProtocol.Twitter
{
    public class TimelineResponseFilterTest
    {
        [Fact]
        public void Run_FilterByNoRetweetUser_NormalTweetTest()
        {
            var accountState = new TwitterAccountState
            {
                NoRetweetUserIds = new HashSet<TwitterUserId>
                {
                    new("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("111"),
                },
            };

            var filter = new TimelineResponseFilter(accountState);
            var filteredPosts = filter.Run(posts);

            // RT ではないのでフィルタ対象ではない
            Assert.Single(filteredPosts);
            Assert.Equal(new TwitterStatusId("100"), filteredPosts[0].StatusId);
        }

        [Fact]
        public void Run_FilterByNoRetweetUser_RetweetTest()
        {
            var accountState = new TwitterAccountState
            {
                NoRetweetUserIds = new HashSet<TwitterUserId>
                {
                    new("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("222"),
                    RetweetedByUserId = new TwitterUserId("111"),
                },
            };

            var filter = new TimelineResponseFilter(accountState);
            var filteredPosts = filter.Run(posts);

            // NoRetweetUserIds に該当するユーザーによる RT なのでフィルタ対象となる
            Assert.Empty(filteredPosts);
        }

        [Fact]
        public void Run_FilterByNoRetweetUser_OriginalTweetTest()
        {
            var accountState = new TwitterAccountState
            {
                NoRetweetUserIds = new HashSet<TwitterUserId>
                {
                    new("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("111"),
                    RetweetedByUserId = new TwitterUserId("222"),
                },
            };

            var filter = new TimelineResponseFilter(accountState);
            var filteredPosts = filter.Run(posts);

            // RT したユーザーではなく RT 元の発言の投稿者なのでフィルタ対象ではない
            Assert.Single(filteredPosts);
            Assert.Equal(new TwitterStatusId("100"), filteredPosts[0].StatusId);
        }

        [Fact]
        public void Run_FilterByBlockedUser_FilteredTest()
        {
            var accountState = new TwitterAccountState
            {
                BlockedUserIds = new HashSet<PersonId>
                {
                    new TwitterUserId("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("111"),
                },
            };

            var filter = new TimelineResponseFilter(accountState)
            {
                IsHomeTimeline = true,
            };
            var filteredPosts = filter.Run(posts);

            Assert.Empty(filteredPosts);
        }

        [Fact]
        public void Run_FilterByBlockedUser_NotFilteredTest()
        {
            var accountState = new TwitterAccountState
            {
                BlockedUserIds = new HashSet<PersonId>
                {
                    new TwitterUserId("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("222"), // ブロックされたユーザーではない
                },
            };

            var filter = new TimelineResponseFilter(accountState)
            {
                IsHomeTimeline = true,
            };
            var filteredPosts = filter.Run(posts);

            Assert.Single(filteredPosts);
            Assert.Equal(new TwitterStatusId("100"), filteredPosts[0].StatusId);
        }

        [Fact]
        public void Run_FilterByBlockedUser_NotHomeTimelineTest()
        {
            var accountState = new TwitterAccountState
            {
                BlockedUserIds = new HashSet<PersonId>
                {
                    new TwitterUserId("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("111"),
                },
            };

            var filter = new TimelineResponseFilter(accountState)
            {
                IsHomeTimeline = false, // ホームタイムライン以外では BlockedUserIds によるフィルタを行わない
            };
            var filteredPosts = filter.Run(posts);

            Assert.Single(filteredPosts);
            Assert.Equal(new TwitterStatusId("100"), filteredPosts[0].StatusId);
        }

        [Fact]
        public void Run_FilterMutedUserPosts_FilteredTest()
        {
            var accountState = new TwitterAccountState
            {
                MutedUserIds = new HashSet<PersonId>
                {
                    new TwitterUserId("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("111"),
                },
            };

            var filter = new TimelineResponseFilter(accountState)
            {
                IsHomeTimeline = true,
            };
            var filteredPosts = filter.Run(posts);

            Assert.Empty(filteredPosts);
        }

        [Fact]
        public void Run_FilterMutedUserPosts_FilteredRetweetTest()
        {
            var accountState = new TwitterAccountState
            {
                MutedUserIds = new HashSet<PersonId>
                {
                    new TwitterUserId("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("222"),
                    RetweetedByUserId = new TwitterUserId("111"), // RT したユーザーもミュート対象となる
                },
            };

            var filter = new TimelineResponseFilter(accountState)
            {
                IsHomeTimeline = true,
            };
            var filteredPosts = filter.Run(posts);

            Assert.Empty(filteredPosts);
        }

        [Fact]
        public void Run_FilterMutedUserPosts_NotFilteredTest()
        {
            var accountState = new TwitterAccountState
            {
                MutedUserIds = new HashSet<PersonId>
                {
                    new TwitterUserId("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("222"), // MutedUserIds にないユーザー
                },
            };

            var filter = new TimelineResponseFilter(accountState)
            {
                IsHomeTimeline = true,
            };
            var filteredPosts = filter.Run(posts);

            Assert.Single(filteredPosts);
            Assert.Equal(new TwitterStatusId("100"), filteredPosts[0].StatusId);
        }

        [Fact]
        public void Run_FilterMutedUserPosts_ReplyTest()
        {
            var accountState = new TwitterAccountState
            {
                MutedUserIds = new HashSet<PersonId>
                {
                    new TwitterUserId("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("111"),
                    IsReply = true, // リプライの場合は MutedUserIds に含まれていてもフィルタ対象外
                },
            };

            var filter = new TimelineResponseFilter(accountState)
            {
                IsHomeTimeline = true,
            };
            var filteredPosts = filter.Run(posts);

            Assert.Single(filteredPosts);
            Assert.Equal(new TwitterStatusId("100"), filteredPosts[0].StatusId);
        }

        [Fact]
        public void Run_FilterMutedUserPosts_NotHomeTimelineTest()
        {
            var accountState = new TwitterAccountState
            {
                MutedUserIds = new HashSet<PersonId>
                {
                    new TwitterUserId("111"),
                },
            };
            var posts = new[]
            {
                new PostClass
                {
                    StatusId = new TwitterStatusId("100"),
                    UserId = new TwitterUserId("111"),
                },
            };

            var filter = new TimelineResponseFilter(accountState)
            {
                IsHomeTimeline = false, // ホームタイムライン以外では MutedUserIds によるフィルタを行わない
            };
            var filteredPosts = filter.Run(posts);

            Assert.Single(filteredPosts);
            Assert.Equal(new TwitterStatusId("100"), filteredPosts[0].StatusId);
        }
    }
}
