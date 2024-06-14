// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2013      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System.Collections.Generic;
using System.Linq;
using OpenTween.Models;

namespace OpenTween.SocialProtocol.Twitter
{
    public class TimelineResponseFilter
    {
        private readonly TwitterAccountState accountState;

        public bool IsHomeTimeline { get; set; }

        public bool IncludeRts { get; set; } = true;

        public TimelineResponseFilter(TwitterAccountState accountState)
        {
            this.accountState = accountState;
        }

        public PostClass[] Run(PostClass[] posts)
        {
            var filteredPosts = posts.AsEnumerable();

            filteredPosts = this.FilterNoRetweetUserPosts(filteredPosts);

            if (this.IsHomeTimeline)
            {
                filteredPosts = this.FilterBlockedUserPosts(filteredPosts);
                filteredPosts = this.FilterMutedUserPosts(filteredPosts);
            }

            if (!this.IncludeRts)
                filteredPosts = this.FilterRetweets(filteredPosts);

            return filteredPosts.ToArray();
        }

        private IEnumerable<PostClass> FilterNoRetweetUserPosts(IEnumerable<PostClass> posts)
            => posts.Where(x => x.RetweetedByUserId == null || !this.accountState.NoRetweetUserIds.Contains(x.RetweetedByUserId));

        private IEnumerable<PostClass> FilterBlockedUserPosts(IEnumerable<PostClass> posts)
            => posts.Where(x => !this.accountState.BlockedUserIds.Contains(x.UserId));

        private IEnumerable<PostClass> FilterMutedUserPosts(IEnumerable<PostClass> posts)
            => posts.Where(x => !this.IsMutedPost(x));

        private bool IsMutedPost(PostClass post)
        {
            // Twitter 標準のミュート機能に準じた判定
            // 参照: https://support.twitter.com/articles/20171399-muting-users-on-twitter

            // リプライはミュート対象外
            if (post.IsReply)
                return false;

            if (this.accountState.MutedUserIds.Contains(post.UserId))
                return true;

            if (post.RetweetedByUserId != null && this.accountState.MutedUserIds.Contains(post.RetweetedByUserId))
                return true;

            return false;
        }

        private IEnumerable<PostClass> FilterRetweets(IEnumerable<PostClass> posts)
            => posts.Where(x => x.RetweetedId == null);
    }
}
