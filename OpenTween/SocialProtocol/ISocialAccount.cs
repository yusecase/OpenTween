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
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.SocialProtocol
{
    public interface ISocialAccount : IDisposable
    {
        public string AccountType { get; }

        public AccountKey UniqueKey { get; }

        public PersonId UserId { get; }

        public string UserName { get; }

        public IApiConnection Connection { get; }

        public ISocialProtocolClient Client { get; }

        public ISocialAccountState AccountState { get; }

        public bool IsDisposed { get; }

        public void Initialize(UserAccount accountSettings, SettingCommon settingCommon);

        public bool CanUsePostId(PostId postId);
    }
}
