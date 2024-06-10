// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Api;

namespace OpenTween.SocialProtocol.Twitter
{
    public partial class TwitterCookieSetupDialog : OTBaseForm, IAccountSetup
    {
        public TwitterCookieSetup Model { get; } = new();

        public Func<IWin32Window?, Uri, Task>? OpenInBrowser { get; set; }

        public TwitterCookieSetupDialog()
        {
            this.InitializeComponent();
            this.InitializeBinding();
        }

        private void InitializeBinding()
        {
            this.bindingSource.DataSource = this.Model;

            this.textBoxTwitterComCookie.DataBindings.Add(
                nameof(TextBox.Text),
                this.bindingSource,
                nameof(TwitterCookieSetup.TwitterComCookie)
            );
        }

        public UserAccount? ShowAccountSetupDialog(IWin32Window? owner)
        {
            var ret = this.ShowDialog(owner);
            if (ret != DialogResult.OK)
                return null;

            return this.Model.AuthorizedAccount!;
        }

        private async void ButtonOK_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNullOrEmpty(this.Model.TwitterComCookie))
                return;

            using (ControlTransaction.Disabled(this))
            {
                try
                {
                    await this.Model.DoAuthorize();

                    this.DialogResult = DialogResult.OK;
                }
                catch (WebApiException ex)
                {
                    this.ShowAuthErrorMessage(ex);
                }
            }
        }

        private void ShowAuthErrorMessage(WebApiException ex)
        {
            var errorBody = ex is TwitterApiException twError
                ? string.Join(Environment.NewLine, twError.LongMessages)
                : ex.Message;

            var message = Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + errorBody;
            MessageBox.Show(this, message, "Authorize", MessageBoxButtons.OK);
        }
    }
}
