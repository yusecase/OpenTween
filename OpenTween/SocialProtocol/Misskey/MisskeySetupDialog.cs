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
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween.SocialProtocol.Misskey
{
    public partial class MisskeySetupDialog : OTBaseForm, IAccountSetup
    {
        public MisskeySetup Model { get; } = new();

        public Func<IWin32Window?, Uri, Task>? OpenInBrowser { get; set; }

        public MisskeySetupDialog()
        {
            this.InitializeComponent();
            this.InitializeBinding();
        }

        private void InitializeBinding()
        {
            this.bindingSource.DataSource = this.Model;

            this.textBoxServerHostname.DataBindings.Add(
                nameof(TextBox.Text),
                this.bindingSource,
                nameof(MisskeySetup.ServerHostname)
            );

            this.groupBoxAuthorize.DataBindings.Add(
                nameof(GroupBox.Enabled),
                this.bindingSource,
                nameof(MisskeySetup.AcquireAuthorize)
            );

            this.linkLabelAuthorize.DataBindings.Add(
                nameof(LinkLabel.Text),
                this.bindingSource,
                nameof(MisskeySetup.AuthorizeUri)
            );
        }

        public UserAccount? ShowAccountSetupDialog(IWin32Window? owner)
        {
            var ret = this.ShowDialog(owner);
            if (ret != DialogResult.OK)
                return null;

            return this.Model.AuthorizedAccount!;
        }

        private void ButtonGetAuthorizeUri_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this))
            {
                try
                {
                    this.Model.GetAuthorizeUri();
                }
                catch (WebApiException ex)
                {
                    this.ShowAuthErrorMessage(ex);
                }
            }

            if (this.Model.AcquireAuthorize)
                this.groupBoxAuthorize.Focus();
        }

        private async void LinkLabelAuthorize_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.Model.AuthorizeUri == null)
                return;

            // 右クリックの場合は無視する
            if (e.Button == MouseButtons.Right)
                return;

            if (this.OpenInBrowser == null)
                throw new InvalidOperationException($"{nameof(this.OpenInBrowser)} is not set");

            await this.OpenInBrowser(this, this.Model.AuthorizeUri);
        }

        private void MenuItemCopyLink_Click(object sender, EventArgs e)
        {
            if (this.Model.AuthorizeUri == null)
                return;

            try
            {
                Clipboard.SetText(this.Model.AuthorizeUri.ToString());
            }
            catch (ExternalException)
            {
            }
        }

        private async void ButtonGetAccessToken_Click(object sender, EventArgs e)
        {
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
            var message = Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + ex.Message;
            MessageBox.Show(this, message, "Authorize", MessageBoxButtons.OK);
        }
    }
}
