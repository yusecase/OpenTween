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
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.SocialProtocol.Misskey;
using OpenTween.SocialProtocol.Twitter;

namespace OpenTween.SocialProtocol
{
    public class AccountSetupDispatcher
    {
        public record AccountSetupItem(
            string Caption,
            Func<IAccountSetup> CreateInstance
        )
        {
            public Guid Id { get; } = Guid.NewGuid();
        }

        private readonly List<AccountSetupItem> setupList;

        public AccountSetupDispatcher()
        {
            this.setupList = new()
            {
                new("Twitter (OAuth)", () => new TwitterOAuthSetupDialog()),
                new("Twitter (Cookie)", () => new TwitterCookieSetupDialog()),
                new("Misskey", () => new MisskeySetupDialog()),
            };
        }

        public (Guid Id, string Caption)[] GetCaptions()
            => this.setupList.Select(x => (x.Id, x.Caption)).ToArray();

        public UserAccount? Dispatch(IWin32Window? owner, Guid setupId, Func<IWin32Window?, Uri, Task>? openInBrowser)
        {
            var setupItem = this.setupList.First(x => x.Id == setupId);

            using var setup = setupItem.CreateInstance();
            setup.OpenInBrowser = openInBrowser;

            return setup.ShowAccountSetupDialog(owner);
        }
    }
}
