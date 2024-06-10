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

using System.Linq;
using System.Threading.Tasks;
using OpenTween.Api.Misskey;
using OpenTween.Models;

namespace OpenTween.SocialProtocol.Misskey
{
    public class MisskeyClient : ISocialProtocolClient
    {
        public static int MaxNoteTextLength { get; } = 3_000;

        private readonly MisskeyAccount account;
        private readonly MisskeyPostFactory postFactory = new();

        public MisskeyClient(MisskeyAccount account)
            => this.account = account;

        public async Task<UserInfo> VerifyCredentials()
        {
            var request = new MeRequest();
            var user = await request.Send(this.account.Connection)
                .ConfigureAwait(false);

            this.account.AccountState.UpdateFromUser(user);

            return new();
        }

        public async Task<PostClass> GetPostById(PostId postId, bool firstLoad)
        {
            var request = new NoteShowRequest
            {
                NoteId = this.AssertMisskeyNoteId(postId),
            };

            var note = await request.Send(this.account.Connection)
                .ConfigureAwait(false);

            var post = this.CreatePostFromNote(note, firstLoad);

            return post;
        }

        public async Task<TimelineResponse> GetHomeTimeline(int count, IQueryCursor? cursor, bool firstLoad)
        {
            var (sinceId, untilId) = GetCursorParams(cursor);
            var request = new NoteTimelineRequest
            {
                Limit = 100,
                SinceId = sinceId,
                UntilId = untilId,
            };

            var notes = await request.Send(this.account.Connection)
                .ConfigureAwait(false);

            var (cursorTop, cursorBottom) = GetCursorFromResponse(notes);
            var posts = this.CreatePostFromNote(notes, firstLoad);

            return new(posts, cursorTop, cursorBottom);
        }

        public Task<TimelineResponse> GetMentionsTimeline(int count, IQueryCursor? cursor, bool firstLoad)
            => throw this.CreateException();

        public Task<TimelineResponse> GetFavoritesTimeline(int count, IQueryCursor? cursor, bool firstLoad)
            => throw this.CreateException();

        public Task<TimelineResponse> GetListTimeline(long listId, int count, IQueryCursor? cursor, bool firstLoad)
            => throw this.CreateException();

        public Task<TimelineResponse> GetSearchTimeline(string query, string lang, int count, IQueryCursor? cursor, bool firstLoad)
            => throw this.CreateException();

        public Task<PostClass[]> GetRelatedPosts(PostClass targetPost, bool firstLoad)
            => throw this.CreateException();

        public async Task<PostClass?> CreatePost(PostStatusParams postParams)
        {
            if (postParams.Text.StartsWith("D "))
                throw this.CreateException(); // DM の送信は非対応

            var request = new NoteCreateRequest
            {
                Text = postParams.Text,
                ReplyId = postParams.InReplyTo is { } replyTo
                    ? this.AssertMisskeyNoteId(replyTo.StatusId)
                    : null,
            };

            var note = await request.Send(this.account.Connection)
                .ConfigureAwait(false);

            var post = this.CreatePostFromNote(note, firstLoad: false);

            return post;
        }

        public int GetTextLengthRemain(PostStatusParams postParams)
            => MaxNoteTextLength - postParams.Text.ToCodepoints().Count();

        public async Task DeletePost(PostId postId)
        {
            var request = new NoteDeleteRequest
            {
                NoteId = (MisskeyNoteId)postId,
            };

            await request.Send(this.account.Connection)
                .ConfigureAwait(false);
        }

        public async Task FavoritePost(PostId postId)
        {
            var request = new NoteReactionCreateRequest
            {
                NoteId = this.AssertMisskeyNoteId(postId),
                Reaction = "❤",
            };

            await request.Send(this.account.Connection)
                .ConfigureAwait(false);
        }

        public async Task UnfavoritePost(PostId postId)
        {
            var request = new NoteReactionDeleteRequest
            {
                NoteId = this.AssertMisskeyNoteId(postId),
            };

            await request.Send(this.account.Connection)
                .ConfigureAwait(false);
        }

        public async Task<PostClass?> RetweetPost(PostId postId)
        {
            var request = new NoteCreateRequest
            {
                RenoteId = this.AssertMisskeyNoteId(postId),
            };

            var note = await request.Send(this.account.Connection)
                .ConfigureAwait(false);

            var post = this.CreatePostFromNote(note, firstLoad: false);

            return post;
        }

        public async Task UnretweetPost(PostId postId)
        {
            var request = new NoteUnrenoteRequest
            {
                NoteId = this.AssertMisskeyNoteId(postId),
            };

            await request.Send(this.account.Connection)
                .ConfigureAwait(false);
        }

        public Task RefreshConfiguration()
            => Task.CompletedTask;

        private WebApiException CreateException()
            => new("Not implemented");

        private MisskeyNoteId AssertMisskeyNoteId(PostId postId)
        {
            return postId is MisskeyNoteId noteId
                ? noteId
                : throw new WebApiException($"Not supported type: {postId.GetType()}");
        }

        private PostClass CreatePostFromNote(MisskeyNote note, bool firstLoad)
            => this.postFactory.CreateFromNote(note, this.account.AccountState, firstLoad);

        private PostClass[] CreatePostFromNote(MisskeyNote[] notes, bool firstLoad)
            => notes.Select(x => this.postFactory.CreateFromNote(x, this.account.AccountState, firstLoad)).ToArray();

        public static (MisskeyNoteId? SinceId, MisskeyNoteId? UntilId) GetCursorParams(IQueryCursor? cursor)
        {
            MisskeyNoteId? sinceId = null, untilId = null;

            if (cursor is QueryCursor<MisskeyNoteId> noteIdCursor)
            {
                if (noteIdCursor.Type == CursorType.Top)
                    sinceId = noteIdCursor.Value;
                else if (noteIdCursor.Type == CursorType.Bottom)
                    untilId = noteIdCursor.Value;
            }

            return (sinceId, untilId);
        }

        public static (IQueryCursor? CursorTop, IQueryCursor? CursorBottom) GetCursorFromResponse(MisskeyNote[] notes)
        {
            IQueryCursor? cursorTop = null, cursorBottom = null;

            if (notes.Length > 0)
            {
                var (min, max) = notes.Select(x => new MisskeyNoteId(x.Id)).MinMax();
                cursorTop = new QueryCursor<MisskeyNoteId>(CursorType.Top, max);
                cursorBottom = new QueryCursor<MisskeyNoteId>(CursorType.Bottom, min);
            }

            return (cursorTop, cursorBottom);
        }
    }
}
