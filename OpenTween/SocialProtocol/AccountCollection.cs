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
using System.Linq;
using OpenTween.Models;
using OpenTween.SocialProtocol.Twitter;

namespace OpenTween.SocialProtocol
{
    public sealed class AccountCollection : IDisposable
    {
        private Dictionary<AccountKey, ISocialAccount> accounts = new();
        private AccountKey? primaryAccountKey;
        private readonly ISocialAccount emptyAccount = new TwitterAccount(AccountKey.Empty);

        public bool IsDisposed { get; private set; }

        public ISocialAccount Primary
            => this.primaryAccountKey != null ? this.accounts[this.primaryAccountKey.Value] : this.emptyAccount;

        public ISocialAccount[] Items
            => this.accounts.Values.ToArray();

        public ISocialAccount[] SecondaryAccounts
            => this.accounts.Values.Where(x => x.UniqueKey != this.primaryAccountKey).ToArray();

        public void LoadFromSettings(SettingCommon settingCommon)
        {
            var factory = new AccountFactory();
            var oldAccounts = this.accounts;
            var newAccounts = new Dictionary<AccountKey, ISocialAccount>();

            foreach (var accountSettings in settingCommon.UserAccounts)
            {
                if (accountSettings.Disabled)
                    continue;

                var accountKey = new AccountKey(accountSettings.UniqueKey);

                if (oldAccounts.TryGetValue(accountKey, out var account))
                    account.Initialize(accountSettings, settingCommon);
                else
                    account = factory.Create(accountSettings, settingCommon);

                newAccounts[accountKey] = account;
            }

            this.accounts = newAccounts;
            this.primaryAccountKey = settingCommon.SelectedAccountKey is { } guid ? new(guid) : null;

            var removedAccounts = oldAccounts
                .Where(x => !newAccounts.ContainsKey(x.Key))
                .Select(x => x.Value);

            this.DisposeAccounts(removedAccounts);
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.emptyAccount.Dispose();
            this.DisposeAccounts(this.accounts.Values);

            this.IsDisposed = true;
        }

        private void DisposeAccounts(IEnumerable<ISocialAccount> accounts)
        {
            foreach (var account in accounts)
                account.Dispose();
        }

        public ISocialAccount GetAccountForTab(TabModel tab)
        {
            if (tab.SourceAccountKey is { } accountKey)
            {
                if (this.accounts.TryGetValue(accountKey, out var account))
                    return account;

                // タブ追加後に設定画面からアカウントの情報を削除した場合
                return new InvalidAccount(accountKey);
            }

            return this.Primary;
        }

        public ISocialAccount? GetAccountForPostId(PostId postId, AccountKey? preferredAccountKey)
        {
            if (preferredAccountKey != null && this.accounts.TryGetValue(preferredAccountKey.Value, out var preferredAccount))
            {
                if (preferredAccount.CanUsePostId(postId))
                    return preferredAccount;
            }

            var primaryAccount = this.Primary;
            if (primaryAccount.CanUsePostId(postId))
                return primaryAccount;

            foreach (var account in this.SecondaryAccounts)
            {
                if (account.CanUsePostId(postId))
                    return account;
            }

            return null;
        }
    }
}
