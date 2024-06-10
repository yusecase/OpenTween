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
using OpenTween.SocialProtocol;

namespace OpenTween.Models
{
    public class HomeSpecifiedAccountTabModel : InternalStorageTabModel
    {
        public override MyCommon.TabUsageType TabType
            => MyCommon.TabUsageType.Undefined;

        public override bool IsPermanentTabType
            => false;

        public override AccountKey? SourceAccountKey { get; }

        public HomeSpecifiedAccountTabModel(string tabName, AccountKey accountKey)
            : base(tabName)
        {
            this.SourceAccountKey = accountKey;
        }

        public override async Task RefreshAsync(ISocialAccount account, bool backward, IProgress<string> progress)
        {
            progress.Report("Home refreshing...");

            var firstLoad = !this.IsFirstLoadCompleted;
            var count = Twitter.GetApiResultCount(MyCommon.WORKERTYPE.Timeline, backward, firstLoad);
            var cursor = backward ? this.CursorBottom : this.CursorTop;

            var response = await account.Client.GetHomeTimeline(count, cursor, firstLoad)
                .ConfigureAwait(false);

            foreach (var post in response.Posts)
                this.AddPostQueue(post);

            TabInformations.GetInstance().DistributePosts();

            if (response.CursorTop != null && !backward)
                this.CursorTop = response.CursorTop;

            if (response.CursorBottom != null)
                this.CursorBottom = response.CursorBottom;

            if (firstLoad)
                this.IsFirstLoadCompleted = true;

            progress.Report("Home refreshed");
        }
    }
}
