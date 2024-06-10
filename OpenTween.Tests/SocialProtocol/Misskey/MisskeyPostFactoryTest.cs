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

using OpenTween.Api.Misskey;
using OpenTween.Models;
using Xunit;

namespace OpenTween.SocialProtocol.Misskey
{
    public class MisskeyPostFactoryTest
    {
        [Fact]
        public void CreateFromNote_LocalNoteTest()
        {
            var note = new MisskeyNote
            {
                Id = "abcdef",
                CreatedAt = "2024-01-01T01:02:03.456Z",
                Text = "foo",
                User = new()
                {
                    Id = "ghijkl",
                    Username = "bar",
                },
                Visibility = "public",
            };
            var factory = new MisskeyPostFactory(new());
            var accountState = new MisskeyAccountState(new("https://example.com/"), new("aaaa"), "hoge");

            var post = factory.CreateFromNote(note, accountState, firstLoad: false);
            Assert.Equal(new MisskeyNoteId("abcdef"), post.StatusId);
            Assert.Equal(new(2024, 1, 1, 1, 2, 3, 456), post.CreatedAtForSorting);
            Assert.Equal(new(2024, 1, 1, 1, 2, 3, 456), post.CreatedAt);
            Assert.Equal(new("https://example.com/notes/abcdef"), post.PostUri);
            Assert.Equal("foo", post.Text);
            Assert.Equal("foo", post.TextFromApi);
            Assert.Equal("foo", post.AccessibleText);
            Assert.Empty(post.QuoteStatusIds);
            Assert.False(post.IsFav);
            Assert.False(post.IsReply);
            Assert.Null(post.InReplyToStatusId);
            Assert.Null(post.InReplyToUser);
            Assert.Null(post.InReplyToUserId);
            Assert.False(post.IsProtect);
            Assert.Equal(new MisskeyUserId("ghijkl"), post.UserId);
            Assert.Equal("bar", post.ScreenName);
            Assert.Equal("bar", post.Nickname);
            Assert.Equal("", post.ImageUrl);
            Assert.Null(post.RetweetedId);
            Assert.Null(post.RetweetedBy);
            Assert.Null(post.RetweetedByUserId);
            Assert.False(post.IsRead);
        }

        [Fact]
        public void CreateFromNote_RemoteNoteTest()
        {
            var note = new MisskeyNote
            {
                Id = "abcdef",
                CreatedAt = "2024-01-01T01:02:03.456Z",
                Text = "foo",
                User = new()
                {
                    Id = "ghijkl",
                    Username = "bar",
                    Host = "bbb.example.com",
                },
                Visibility = "public",
            };
            var factory = new MisskeyPostFactory(new());
            var accountState = new MisskeyAccountState(new("https://aaa.example.com/"), new("aaaa"), "hoge");

            var post = factory.CreateFromNote(note, accountState, firstLoad: false);
            Assert.Equal(new("https://aaa.example.com/notes/abcdef"), post.PostUri);
            Assert.Equal("bar@bbb.example.com", post.ScreenName);
        }

        [Fact]
        public void CreateFromNote_ReplyTest()
        {
            var repliedNote = new MisskeyNote
            {
                Id = "aaaaa",
                CreatedAt = "2023-12-31T00:00:00.000Z",
                Text = "hoge",
                UserId = "abcdef",
                User = new()
                {
                    Id = "abcdef",
                    Username = "foo",
                },
                Visibility = "public",
            };
            var note = new MisskeyNote
            {
                Id = "bbbbb",
                CreatedAt = "2024-01-01T01:02:03.456Z",
                Text = "@foo tetete",
                User = new()
                {
                    Id = "ghijkl",
                    Username = "bar",
                },
                Reply = repliedNote,
                Visibility = "public",
            };
            var factory = new MisskeyPostFactory(new());
            var accountState = new MisskeyAccountState(new("https://example.com/"), new("aaaa"), "hoge");

            var post = factory.CreateFromNote(note, accountState, firstLoad: false);
            Assert.Equal(new MisskeyNoteId("bbbbb"), post.StatusId);
            Assert.Equal("@foo tetete", post.Text);
            Assert.Equal(new MisskeyUserId("ghijkl"), post.UserId);
            Assert.Equal("bar", post.ScreenName);
            Assert.Equal(new MisskeyNoteId("aaaaa"), post.InReplyToStatusId);
            Assert.Equal("foo", post.InReplyToUser);
            Assert.Equal(new MisskeyUserId("abcdef"), post.InReplyToUserId);
        }

        [Fact]
        public void CreateFromNote_RenoteTest()
        {
            var renotedNote = new MisskeyNote
            {
                Id = "aaaaa",
                CreatedAt = "2023-12-31T00:00:00.000Z",
                Text = "hoge",
                User = new()
                {
                    Id = "abcdef",
                    Username = "foo",
                },
                Visibility = "public",
            };
            var note = new MisskeyNote
            {
                Id = "bbbbb",
                CreatedAt = "2024-01-01T01:02:03.456Z",
                Text = null,
                User = new()
                {
                    Id = "ghijkl",
                    Username = "bar",
                },
                Renote = renotedNote,
                Visibility = "public",
            };
            var factory = new MisskeyPostFactory(new());
            var accountState = new MisskeyAccountState(new("https://example.com/"), new("aaaa"), "hoge");

            var post = factory.CreateFromNote(note, accountState, firstLoad: false);
            Assert.Equal(new MisskeyNoteId("bbbbb"), post.StatusId);
            Assert.Equal(new(2024, 1, 1, 1, 2, 3, 456), post.CreatedAtForSorting);
            Assert.Equal(new(2023, 12, 31, 0, 0, 0, 0), post.CreatedAt);
            Assert.Equal(new("https://example.com/notes/aaaaa"), post.PostUri);
            Assert.Equal("hoge", post.Text);
            Assert.Equal(new MisskeyUserId("abcdef"), post.UserId);
            Assert.Equal("foo", post.ScreenName);
            Assert.Equal(new MisskeyNoteId("aaaaa"), post.RetweetedId);
            Assert.Equal("bar", post.RetweetedBy);
            Assert.Equal(new MisskeyUserId("ghijkl"), post.RetweetedByUserId);
        }

        [Fact]
        public void CreateFromNote_QuotedNoteTest()
        {
            var quotedNote = new MisskeyNote
            {
                Id = "aaaaa",
                CreatedAt = "2023-12-31T00:00:00.000Z",
                Text = "hoge",
                User = new()
                {
                    Id = "abcdef",
                    Username = "foo",
                },
                Visibility = "public",
            };
            var note = new MisskeyNote
            {
                Id = "bbbbb",
                CreatedAt = "2024-01-01T01:02:03.456Z",
                Text = "tetete", // Text が空でなく Renote がある場合は引用とみなす
                User = new()
                {
                    Id = "ghijkl",
                    Username = "bar",
                },
                Renote = quotedNote,
                Visibility = "public",
            };
            var factory = new MisskeyPostFactory(new());
            var accountState = new MisskeyAccountState(new("https://example.com/"), new("aaaa"), "hoge");

            var post = factory.CreateFromNote(note, accountState, firstLoad: false);
            Assert.Equal(new MisskeyNoteId("bbbbb"), post.StatusId);
            Assert.Equal(new(2024, 1, 1, 1, 2, 3, 456), post.CreatedAtForSorting);
            Assert.Equal(new(2024, 1, 1, 1, 2, 3, 456), post.CreatedAt);
            Assert.Equal(new("https://example.com/notes/bbbbb"), post.PostUri);
            Assert.Equal("tetete", post.Text);
            Assert.Equal(new MisskeyUserId("ghijkl"), post.UserId);
            Assert.Equal("bar", post.ScreenName);
            Assert.Null(post.RetweetedId);
            Assert.Null(post.RetweetedBy);
            Assert.Null(post.RetweetedByUserId);
            Assert.Equal(new[] { new MisskeyNoteId("aaaaa") }, post.QuoteStatusIds);
        }

        [Fact]
        public void CreateFromNote_TextContainsTwitterUrlTest()
        {
            var note = new MisskeyNote
            {
                Id = "aaaaa",
                CreatedAt = "2023-12-31T00:00:00.000Z",
                Text = "hoge https://twitter.com/hoge/status/12345",
                User = new()
                {
                    Id = "abcdef",
                    Username = "foo",
                },
                Visibility = "public",
            };
            var factory = new MisskeyPostFactory(new());
            var accountState = new MisskeyAccountState(new("https://example.com/"), new("aaaa"), "hoge");

            var post = factory.CreateFromNote(note, accountState, firstLoad: false);
            Assert.Equal(new MisskeyNoteId("aaaaa"), post.StatusId);
            Assert.Equal(new[] { new TwitterStatusId("12345") }, post.QuoteStatusIds);
        }
    }
}
