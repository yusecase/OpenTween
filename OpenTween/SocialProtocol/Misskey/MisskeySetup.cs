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
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using OpenTween.Api.Misskey;
using OpenTween.Connection;

namespace OpenTween.SocialProtocol.Misskey
{
    public class MisskeySetup : NotifyPropertyChangedBase
    {
        public static readonly string[] AuthorizeScopes = new[]
        {
            "read:account",
            "write:notes",
            "write:reactions",
        };

        public string ServerHostname
        {
            get => this.serverHostname;
            set => this.SetProperty(ref this.serverHostname, value);
        }

        public bool AcquireAuthorize
        {
            get => this.acquireAuthorize;
            private set => this.SetProperty(ref this.acquireAuthorize, value);
        }

        public Uri? AuthorizeUri
        {
            get => this.authorizeUri;
            private set => this.SetProperty(ref this.authorizeUri, value);
        }

        public UserAccount? AuthorizedAccount { get; private set; }

        private string serverHostname = "";
        private Uri? serverBaseUri;
        private Guid sessionNonce = Guid.NewGuid();
        private bool acquireAuthorize = false;
        private Uri? authorizeUri;

        public void GetAuthorizeUri()
        {
            this.UpdateServerBaseUri();
            this.sessionNonce = Guid.NewGuid();

            var path = $"/miauth/{Uri.EscapeDataString(this.sessionNonce.ToString())}";
            var query = MyCommon.BuildQueryString(new KeyValuePair<string, string>[]
            {
                new("name", ApplicationSettings.ApplicationName),
                new("permission", string.Join(",", AuthorizeScopes)),
            });
            var authorizeUri = new Uri(this.serverBaseUri, $"{path}?{query}");

            this.GotoAuthorizeStep(authorizeUri);
        }

        [MemberNotNull(nameof(serverBaseUri))]
        private void UpdateServerBaseUri()
        {
            try
            {
                var parsedUri = new Uri($"https://{this.serverHostname}", UriKind.Absolute);
                this.serverBaseUri = new(parsedUri.GetLeftPart(UriPartial.Authority));
            }
            catch (UriFormatException ex)
            {
                throw new WebApiException($"Invalid server name: {this.serverHostname}", ex);
            }
        }

        private void GotoAuthorizeStep(Uri authorizeUri)
        {
            this.authorizeUri = authorizeUri;
            this.AcquireAuthorize = true;
        }

        public async Task DoAuthorize()
        {
            if (this.serverBaseUri == null)
                throw new InvalidOperationException($"{nameof(this.serverBaseUri)} is null");

            var apiBaseUri = new Uri(this.serverBaseUri, "/api/");
            var apiConnection = new MisskeyApiConnection(apiBaseUri, accessToken: "");
            var request = new MiauthCheckRequest
            {
                SessionNonce = this.sessionNonce.ToString(),
            };

            var tokenResponse = await request.Send(apiConnection)
                .ConfigureAwait(false);

            if (tokenResponse == null)
                return;

            this.AuthorizedAccount = new()
            {
                AccountType = "Misskey",
                ServerHostname = this.serverBaseUri.Host,
                UserId = tokenResponse.User.Id,
                Username = tokenResponse.User.Username,
                TokenSecret = tokenResponse.Token,
                Scopes = AuthorizeScopes,
            };
        }
    }
}
