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

using System.Threading.Tasks;
using OpenTween.Models;

namespace OpenTween.SocialProtocol
{
    public interface ISocialProtocolClient
    {
        public Task<UserInfo> VerifyCredentials();

        public Task<PostClass> GetPostById(PostId postId, bool firstLoad);

        public Task<TimelineResponse> GetHomeTimeline(int count, IQueryCursor? cursor, bool firstLoad);

        public Task<TimelineResponse> GetMentionsTimeline(int count, IQueryCursor? cursor, bool firstLoad);

        public Task<TimelineResponse> GetFavoritesTimeline(int count, IQueryCursor? cursor, bool firstLoad);

        public Task<TimelineResponse> GetListTimeline(long listId, int count, IQueryCursor? cursor, bool firstLoad);

        public Task<TimelineResponse> GetSearchTimeline(string query, string lang, int count, IQueryCursor? cursor, bool firstLoad);

        public Task<PostClass[]> GetRelatedPosts(PostClass targetPost, bool firstLoad);

        public Task<PostClass?> CreatePost(PostStatusParams postParams);

        public int GetTextLengthRemain(PostStatusParams postParams);

        public Task DeletePost(PostId postId);

        public Task FavoritePost(PostId postId);

        public Task UnfavoritePost(PostId postId);

        public Task<PostClass?> RetweetPost(PostId postId);

        public Task UnretweetPost(PostId postId);

        public Task RefreshConfiguration();
    }
}
