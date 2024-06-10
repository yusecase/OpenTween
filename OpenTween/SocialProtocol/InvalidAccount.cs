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

using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTween.Api;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.SocialProtocol
{
    public class InvalidAccount : ISocialAccount
    {
        public string AccountType
            => "InvalidAccount";

        public AccountKey UniqueKey { get; }

        public PersonId UserId
            => this.AccountState.UserId;

        public string UserName
            => this.AccountState.UserName;

        public IApiConnection Connection { get; } = new InvalidAccountConnection();

        public ISocialProtocolClient Client { get; } = new InvalidAccountClient();

        public ISocialAccountState AccountState { get; } = new InvalidAccountState();

        public bool IsDisposed { get; private set; }

        public InvalidAccount(AccountKey accountKey)
            => this.UniqueKey = accountKey;

        public void Initialize(UserAccount accountSettings, SettingCommon settingCommon)
        {
        }

        public bool CanUsePostId(PostId postId)
            => false;

        public void Dispose()
            => this.IsDisposed = true;

        private class InvalidAccountConnection : IApiConnection
        {
            public Task<ApiResponse> SendAsync(IHttpRequest request)
                => throw new WebApiException("Invalid account");

            public void Dispose()
            {
            }
        }

        private class InvalidAccountClient : ISocialProtocolClient
        {
            public Task<UserInfo> VerifyCredentials()
                => throw this.CreateException();

            public Task<PostClass> GetPostById(PostId postId, bool firstLoad)
                => throw this.CreateException();

            public Task<TimelineResponse> GetHomeTimeline(int count, IQueryCursor? cursor, bool firstLoad)
                => throw this.CreateException();

            public Task<TimelineResponse> GetMentionsTimeline(int count, IQueryCursor? cursor, bool firstLoad)
                => throw this.CreateException();

            public Task<TimelineResponse> GetFavoritesTimeline(int count, IQueryCursor? cursor, bool firstLoad)
                => throw this.CreateException();

            public Task<TimelineResponse> GetListTimeline(long listId, int count, IQueryCursor? cursor, bool firstLoad)
                => throw this.CreateException();

            public Task<TimelineResponse> GetSearchTimeline(string query, string lang, int count, IQueryCursor? cursor, bool firstLoad)
                => throw this.CreateException();

            public Task<PostClass[]> GetRelatedPosts(PostClass targetPost, bool firstLoad)
                => throw this.CreateException();

            public Task<PostClass?> CreatePost(PostStatusParams postParams)
                => throw this.CreateException();

            public int GetTextLengthRemain(PostStatusParams postParams)
                => 0;

            public Task DeletePost(PostId postId)
                => throw this.CreateException();

            public Task FavoritePost(PostId postId)
                => throw this.CreateException();

            public Task UnfavoritePost(PostId postId)
                => throw this.CreateException();

            public Task<PostClass?> RetweetPost(PostId postId)
                => throw this.CreateException();

            public Task UnretweetPost(PostId postId)
                => throw this.CreateException();

            public Task RefreshConfiguration()
                => throw this.CreateException();

            private WebApiException CreateException()
                => new("Invalid account");
        }

        private class InvalidAccountState : ISocialAccountState
        {
            public PersonId UserId
                => new TwitterUserId("0");

            public string UserName
                => "(Unknown account)";

            public int? FollowersCount
                => null;

            public int? FriendsCount
                => null;

            public int? StatusesCount
                => null;

            public ISet<PersonId> FollowerIds { get; } = new HashSet<PersonId>();

            public ISet<PersonId> BlockedUserIds { get; } = new HashSet<PersonId>();

            public RateLimitCollection RateLimits { get; } = new();

            public bool HasUnrecoverableError { get; set; } = true;
        }
    }
}
