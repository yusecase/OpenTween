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

using OpenTween.Models;
using Xunit;

namespace OpenTween.SocialProtocol.Twitter
{
    public class TwitterAccountTest
    {
        [Fact]
        public void Initialize_Test()
        {
            var accountKey = AccountKey.New();
            using var account = new TwitterAccount(accountKey);

            var accountSettings = new UserAccount
            {
                UniqueKey = accountKey.Id,
                TwitterAuthType = APIAuthType.OAuth1,
                Token = "aaaaa",
                TokenSecret = "aaaaa",
                UserId = "11111",
                Username = "tetete",
            };
            var settingCommon = new SettingCommon();
            account.Initialize(accountSettings, settingCommon);
            Assert.Equal(new TwitterUserId("11111"), account.UserId);
            Assert.Equal("tetete", account.UserName);
            Assert.Equal(APIAuthType.OAuth1, account.AuthType);
            Assert.Same(account.Legacy.Api.Connection, account.Connection);
        }

        [Fact]
        public void Initialize_ReconfigureTest()
        {
            var accountKey = AccountKey.New();
            using var account = new TwitterAccount(accountKey);

            var accountSettings1 = new UserAccount
            {
                UniqueKey = accountKey.Id,
                TwitterAuthType = APIAuthType.OAuth1,
                Token = "aaaaa",
                TokenSecret = "aaaaa",
                UserId = "11111",
                Username = "tetete",
            };
            var settingCommon1 = new SettingCommon();
            account.Initialize(accountSettings1, settingCommon1);
            Assert.Equal(new TwitterUserId("11111"), account.UserId);

            var accountSettings2 = new UserAccount
            {
                UniqueKey = accountKey.Id,
                TwitterAuthType = APIAuthType.OAuth1,
                Token = "bbbbb",
                TokenSecret = "bbbbb",
                UserId = "22222",
                Username = "hoge",
            };
            var settingCommon2 = new SettingCommon();
            account.Initialize(accountSettings2, settingCommon2);
            Assert.Equal(new TwitterUserId("22222"), account.UserId);
        }

        [Fact]
        public void AccountType_Test()
        {
            using var account = new TwitterAccount(AccountKey.New());
            Assert.Equal("Twitter", account.AccountType);
        }

        [Fact]
        public void Client_V1_Test()
        {
            var accountKey = AccountKey.New();
            using var account = new TwitterAccount(accountKey);

            var accountSettings = new UserAccount
            {
                UniqueKey = accountKey.Id,
                TwitterAuthType = APIAuthType.OAuth1,
                Token = "aaaaa",
                TokenSecret = "aaaaa",
                UserId = "11111",
                Username = "tetete",
            };
            var settingCommon = new SettingCommon();
            account.Initialize(accountSettings, settingCommon);

            Assert.IsType<TwitterV1Client>(account.Client);
        }

        [Fact]
        public void Client_Graphql_Test()
        {
            var accountKey = AccountKey.New();
            using var account = new TwitterAccount(accountKey);

            var accountSettings = new UserAccount
            {
                UniqueKey = accountKey.Id,
                TwitterAuthType = APIAuthType.TwitterComCookie,
                TwitterComCookie = "auth_token=foo; ct0=bar",
                UserId = "11111",
                Username = "tetete",
            };
            var settingCommon = new SettingCommon();
            account.Initialize(accountSettings, settingCommon);

            Assert.IsType<TwitterGraphqlClient>(account.Client);
        }
    }
}
