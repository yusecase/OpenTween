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
using OpenTween.Api;
using OpenTween.Api.Misskey;
using OpenTween.Models;

namespace OpenTween.SocialProtocol.Misskey
{
    public class MisskeyAccountState : ISocialAccountState
    {
        public Uri ServerUri { get; private set; }

        public MisskeyUserId UserId { get; private set; }

        PersonId ISocialAccountState.UserId
            => this.UserId;

        public string UserName { get; private set; }

        public string[] AuthorizedScopes { get; set; } = Array.Empty<string>();

        public int? FollowersCount { get; private set; }

        public int? FriendsCount { get; private set; }

        public int? StatusesCount { get; private set; }

        public ISet<PersonId> FollowerIds { get; set; } = new HashSet<PersonId>();

        public ISet<PersonId> BlockedUserIds { get; set; } = new HashSet<PersonId>();

        public RateLimitCollection RateLimits { get; } = new();

        public bool HasUnrecoverableError { get; set; } = true;

        public MisskeyAccountState()
            : this(null!, new("0"), "")
        {
        }

        public MisskeyAccountState(Uri serverUri, MisskeyUserId userId, string userName)
        {
            this.ServerUri = serverUri;
            this.UserId = userId;
            this.UserName = userName;
        }

        public void UpdateFromSettings(UserAccount accountSettings)
        {
            this.ServerUri = new($"https://{accountSettings.ServerHostname}/");
            this.UserId = new(accountSettings.UserId);
            this.UserName = accountSettings.Username;
            this.AuthorizedScopes = accountSettings.Scopes;
        }

        /// <summary>ユーザー情報を更新します</summary>
        public void UpdateFromUser(MisskeyUser self)
        {
            this.UserId = new(self.Id);
            this.UserName = self.Username;
            this.FollowersCount = self.FollowersCount;
            this.FriendsCount = self.FollowingCount;
            this.StatusesCount = self.NotesCount;
        }
    }
}
