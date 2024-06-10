// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Windows.Forms;

namespace OpenTween.Controls
{
    public partial class PublicSearchHeaderPanel : UserControl
    {
        public string Query
        {
            get => this.comboBoxQuery.Text;
            set => this.InsertQueryHistory(value);
        }

        public string Lang
        {
            get => this.comboBoxLang.Text;
            set => this.comboBoxLang.Text = value;
        }

        public event EventHandler<EventArgs>? EscKeyDown;

        public event EventHandler<EventArgs>? Search;

        private bool layoutFixed = false;

        public PublicSearchHeaderPanel()
            => this.InitializeComponent();

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            this.FixLayoutWorkaround();
        }

        public void FixLayoutWorkaround()
        {
            if (this.layoutFixed)
                return;

            // ComboBox が正しく上下中央揃えにならない現象の回避策
            using (ControlTransaction.Layout(this.tableLayoutPanel1))
            {
                var height = this.comboBoxQuery.Height;
                this.buttonSearch.Height = height;
                this.comboBoxQuery.Height += 1;
                this.comboBoxQuery.Height = height;
                this.comboBoxLang.Height += 1;
                this.comboBoxLang.Height = height;
            }

            this.layoutFixed = true;
        }

        public void FocusToQuery()
            => this.comboBoxQuery.Focus();

        private void DoSearch()
        {
            this.Search?.Invoke(this, EventArgs.Empty);

            var query = this.Query.Trim();
            this.InsertQueryHistory(query);
        }

        private void InsertQueryHistory(string query)
        {
            if (MyCommon.IsNullOrEmpty(query))
                return;

            var index = this.comboBoxQuery.Items.IndexOf(query);
            if (index != -1)
                this.comboBoxQuery.Items.RemoveAt(index);
            this.comboBoxQuery.Items.Insert(0, query);
            this.comboBoxQuery.Text = query;
            this.comboBoxQuery.SelectAll();
        }

        private void EnableTabStop()
        {
            this.comboBoxQuery.TabStop = true;
            this.comboBoxLang.TabStop = true;
            this.buttonSearch.TabStop = true;
        }

        private void DisableTabStop()
        {
            this.comboBoxQuery.TabStop = false;
            this.comboBoxLang.TabStop = false;
            this.buttonSearch.TabStop = false;
        }

        private void TableLayoutPanel1_Enter(object sender, EventArgs e)
            => this.EnableTabStop();

        private void TableLayoutPanel1_Leave(object sender, EventArgs e)
            => this.DisableTabStop();

        private void ComboBoxQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                this.EscKeyDown?.Invoke(this, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.DoSearch();
            }
        }

        private void ComboBoxLang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.DoSearch();
            }
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
            => this.DoSearch();
    }
}
