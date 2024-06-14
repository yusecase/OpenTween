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
using System.Diagnostics;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.SocialProtocol.Misskey
{
    public sealed class MisskeyAccount : ISocialAccount
    {
        public string AccountType
            => "Misskey";

        public AccountKey UniqueKey { get; }

        public MisskeyClient Client { get; private set; }

        ISocialProtocolClient ISocialAccount.Client
            => this.Client;

        public bool IsDisposed { get; private set; }

        public MisskeyAccountState AccountState { get; } = new();

        ISocialAccountState ISocialAccount.AccountState
            => this.AccountState;

        public PersonId UserId
            => this.AccountState.UserId;

        public string UserName
            => this.AccountState.UserName;

        public MisskeyApiConnection Connection
            => this.connection ?? throw new InvalidOperationException("Not initialized");

        IApiConnection ISocialAccount.Connection
            => this.Connection;

        private MisskeyApiConnection? connection;

        public MisskeyAccount(AccountKey accountKey)
        {
            this.UniqueKey = accountKey;
            this.Client = new(this);
        }

        public void Initialize(UserAccount accountSettings, SettingCommon settingCommon)
        {
            Debug.Assert(accountSettings.UniqueKey == this.UniqueKey.Id, "UniqueKey must be same as current value.");

            this.AccountState.UpdateFromSettings(accountSettings);
            this.AccountState.HasUnrecoverableError = false;

            var apiBaseUri = new Uri(this.AccountState.ServerUri, "/api/");

            var newConnection = new MisskeyApiConnection(apiBaseUri, accountSettings.TokenSecret, this.AccountState);
            (this.connection, var oldConnection) = (newConnection, this.connection);
            oldConnection?.Dispose();
        }

        public bool CanUsePostId(PostId postId)
            => postId is MisskeyNoteId;

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;
        }
    }
}
