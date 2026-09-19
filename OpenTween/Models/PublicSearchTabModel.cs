// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTween.SocialProtocol;
using OpenTween.SocialProtocol.Twitter;

namespace OpenTween.Models
{
    public class PublicSearchTabModel : InternalStorageTabModel
    {
        public override MyCommon.TabUsageType TabType
            => MyCommon.TabUsageType.PublicSearch;

        public string SearchWords
        {
            get => this.searchWords;
            set
            {
                this.searchWords = value;
                this.ResetFetchIds();
            }
        }

        public string SearchLang
        {
            get => this.searchLang;
            set
            {
                this.searchLang = value;
                this.ResetFetchIds();
            }
        }

        private string searchWords = "";
        private string searchLang = "";
        private readonly ConcurrentDictionary<PostId, long> detectionBatches = new();
        private long newestDetectionBatch;
        private long oldestDetectionBatch;

        public bool SortByDetectionOrder { get; private set; }

        public bool DetectionOrderDescending { get; private set; } = true;

        public PublicSearchTabModel(string tabName)
            : base(tabName)
        {
        }

        public override async Task RefreshAsync(ISocialAccount account, bool backward, IProgress<string> progress)
        {
            if (MyCommon.IsNullOrEmpty(this.SearchWords))
                return;

            progress.Report("Search refreshing...");

            var firstLoad = !this.IsFirstLoadCompleted;
            var count = TwitterLegacy.GetApiResultCount(MyCommon.WORKERTYPE.PublicSearch, backward, firstLoad);
            var cursor = backward ? this.CursorBottom : this.CursorTop;

            var response = await account.Client.GetSearchTimeline(this.SearchWords, this.SearchLang, count, cursor, firstLoad)
                .ConfigureAwait(false);

            this.AddPostsFromRefresh(response.Posts, backward);

            TabInformations.GetInstance().DistributePosts();

            if (response.CursorTop != null && !backward)
                this.CursorTop = response.CursorTop;

            if (response.CursorBottom != null)
                this.CursorBottom = response.CursorBottom;

            if (firstLoad)
                this.IsFirstLoadCompleted = true;

            progress.Report("Search refreshed");
        }

        internal void AddPostsFromRefresh(IEnumerable<PostClass> posts, bool backward)
        {
            var detectionBatch = backward
                ? --this.oldestDetectionBatch
                : ++this.newestDetectionBatch;

            foreach (var post in posts)
            {
                this.detectionBatches.TryAdd(post.StatusId, detectionBatch);
                this.AddPostQueue(post);
            }
        }

        public void SetSortByDetectionOrder(bool enabled, bool descending = true)
        {
            this.SortByDetectionOrder = enabled;
            this.DetectionOrderDescending = descending;

            if (enabled)
                base.SetSortMode(ComparerMode.Id, descending ? System.Windows.Forms.SortOrder.Descending : System.Windows.Forms.SortOrder.Ascending);
            else
                this.ApplySortMode();
        }

        public void ToggleDetectionSortOrder()
        {
            this.DetectionOrderDescending = !this.DetectionOrderDescending;
            base.SetSortMode(
                ComparerMode.Id,
                this.DetectionOrderDescending ? System.Windows.Forms.SortOrder.Descending : System.Windows.Forms.SortOrder.Ascending
            );
        }

        public override void SetSortMode(ComparerMode mode, System.Windows.Forms.SortOrder sortOrder)
        {
            if (this.SortByDetectionOrder)
            {
                base.SetSortMode(
                    ComparerMode.Id,
                    this.DetectionOrderDescending ? System.Windows.Forms.SortOrder.Descending : System.Windows.Forms.SortOrder.Ascending
                );
                return;
            }

            base.SetSortMode(mode, sortOrder);
        }

        protected override int ComparePosts(PostId xId, PostClass? xPost, PostId yId, PostClass? yPost)
        {
            if (!this.SortByDetectionOrder)
                return base.ComparePosts(xId, xPost, yId, yPost);

            var xBatch = this.detectionBatches.TryGetValue(xId, out var xValue) ? xValue : 0;
            var yBatch = this.detectionBatches.TryGetValue(yId, out var yValue) ? yValue : 0;
            var compare = xBatch.CompareTo(yBatch);
            if (compare != 0)
                return compare;

            return Comparer<DateTimeUtc?>.Default.Compare(xPost?.CreatedAtForSorting, yPost?.CreatedAtForSorting);
        }

        public override void ClearIDs()
        {
            base.ClearIDs();
            this.detectionBatches.Clear();
            this.newestDetectionBatch = 0;
            this.oldestDetectionBatch = 0;
        }

        /// <summary>
        /// 差分更新用の cursor をリセットする（検索条件が変更された時に使用する）
        /// </summary>
        public void ResetFetchIds()
        {
            this.CursorTop = null;
            this.CursorBottom = null;
            this.IsFirstLoadCompleted = false;
        }
    }
}
