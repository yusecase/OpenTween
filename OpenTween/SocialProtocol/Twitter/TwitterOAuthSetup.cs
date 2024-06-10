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
using System.Threading.Tasks;
using OpenTween.Connection;

namespace OpenTween.SocialProtocol.Twitter
{
    public class TwitterOAuthSetup : NotifyPropertyChangedBase
    {
        public bool UseCustomConsumerKey
        {
            get => this.useCustomConsumerKey;
            set => this.SetProperty(ref this.useCustomConsumerKey, value);
        }

        public string CustomConsumerKey
        {
            get => this.customConsumerKey;
            set => this.SetProperty(ref this.customConsumerKey, value);
        }

        public string CustomConsumerSecret
        {
            get => this.customConsumerSecret;
            set => this.SetProperty(ref this.customConsumerSecret, value);
        }

        public Uri? AuthorizeUri
        {
            get => this.authorizeUri;
            private set => this.SetProperty(ref this.authorizeUri, value);
        }

        public bool AcquirePinCode
        {
            get => this.acquirePinCode;
            private set => this.SetProperty(ref this.acquirePinCode, value);
        }

        public string PinCode
        {
            get => this.pinCode;
            set => this.SetProperty(ref this.pinCode, value);
        }

        public UserAccount? AuthorizedAccount { get; private set; }

        private bool useCustomConsumerKey = true;
        private string customConsumerKey = "";
        private string customConsumerSecret = "";
        private TwitterCredentialOAuth1? requestToken;
        private Uri? authorizeUri = null;
        private bool acquirePinCode = false;
        private string pinCode = "";

        public async Task GetAuthorizeUri()
        {
            var appToken = this.GetAppToken();
            var requestToken = await TwitterApiConnection.GetRequestTokenAsync(appToken);
            var authorizeUri = TwitterApiConnection.GetAuthorizeUri(requestToken);

            this.GotoPinCodeStep(requestToken, authorizeUri);
        }

        private TwitterAppToken GetAppToken()
        {
            if (this.UseCustomConsumerKey &&
                !MyCommon.IsNullOrEmpty(this.CustomConsumerKey) &&
                !MyCommon.IsNullOrEmpty(this.CustomConsumerSecret))
            {
                return new()
                {
                    AuthType = APIAuthType.OAuth1,
                    OAuth1CustomConsumerKey = ApiKey.Create(this.CustomConsumerKey),
                    OAuth1CustomConsumerSecret = ApiKey.Create(this.CustomConsumerSecret),
                };
            }

            return TwitterAppToken.GetDefault();
        }

        private void GotoPinCodeStep(TwitterCredentialOAuth1 requestToken, Uri authorizeUri)
        {
            this.requestToken = requestToken;
            this.AuthorizeUri = authorizeUri;
            this.AcquirePinCode = true;
        }

        public async Task DoAuthorize()
        {
            if (this.requestToken == null)
                throw new InvalidOperationException($"{nameof(this.requestToken)} is null");

            if (MyCommon.IsNullOrEmpty(this.PinCode))
                throw new InvalidOperationException($"{nameof(this.PinCode)} is empty");

            var accessTokenResponse = await TwitterApiConnection.GetAccessTokenAsync(this.requestToken, this.PinCode);
            var authorizedAccount = new UserAccount
            {
                AccountType = "Twitter",
                TwitterAuthType = APIAuthType.OAuth1,
                TwitterOAuth1ConsumerKey = this.CustomConsumerKey,
                TwitterOAuth1ConsumerSecret = this.CustomConsumerSecret,
                Username = accessTokenResponse["screen_name"],
                UserId = accessTokenResponse["user_id"],
                Token = accessTokenResponse["oauth_token"],
                TokenSecret = accessTokenResponse["oauth_token_secret"],
            };

            this.AuthorizedAccount = authorizedAccount;
        }
    }
}
