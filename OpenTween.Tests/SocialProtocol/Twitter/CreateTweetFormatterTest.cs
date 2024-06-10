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

using System;
using OpenTween.Models;
using Xunit;

namespace OpenTween.SocialProtocol.Twitter
{
    public class CreateTweetFormatterTest
    {
        [Theory]
        [InlineData("", 280)]
        [InlineData("a", 279)]
        [InlineData("D OpenTween ", 10_000)]
        [InlineData("D OpenTween a", 9_999)]
        [InlineData("hoge https://twitter.com/twitterapi/status/22634515958", 276)]
        public void GetTextLengthRemain_Test(string text, int expected)
        {
            using var twAccount = new TwitterAccount(AccountKey.New());
            var formatter = new CreateTweetFormatter(twAccount);

            var postParams = new PostStatusParams(text);
            Assert.Equal(expected, formatter.GetTextLengthRemain(postParams));
        }

        [Fact]
        public void CreateParams_SimpleTextTest()
        {
            using var twAccount = new TwitterAccount(AccountKey.New());
            var formatter = new CreateTweetFormatter(twAccount);

            var postParams = new PostStatusParams(Text: "hoge");
            var expected = new CreateTweetParams(Text: "hoge");
            Assert.Equal(expected, formatter.CreateParams(postParams));
        }

        [Fact]
        public void CreateParams_RemoveAutoPopuratedMentions_SingleTest()
        {
            using var twAccount = new TwitterAccount(AccountKey.New());

            var formatter = new CreateTweetFormatter(twAccount);
            var inReplyToPost = new PostClass
            {
                StatusId = new TwitterStatusId("1"),
                UserId = new TwitterUserId("12345"),
                ScreenName = "foo",
            };

            // auto_populate_reply_metadata により自動で付与される Mentions を Text から削除する
            var postParams = new PostStatusParams(Text: "@foo hoge", inReplyToPost);
            var expected = new CreateTweetParams(Text: "hoge", inReplyToPost)
            {
                AutoPopulateReplyMetadata = true,
                ExcludeReplyUserIds = Array.Empty<PersonId>(),
            };
            Assert.Equal(expected, formatter.CreateParams(postParams));
        }

        [Fact]
        public void CreateParams_RemoveAutoPopuratedMentions_MultipleTest()
        {
            using var twAccount = new TwitterAccount(AccountKey.New());

            var formatter = new CreateTweetFormatter(twAccount);
            var inReplyToPost = new PostClass
            {
                StatusId = new TwitterStatusId("1"),
                UserId = new TwitterUserId("12345"),
                ScreenName = "foo",
                Text = "@bar tetete",
                ReplyToList = new() { (new TwitterUserId("67890"), "bar") },
            };

            // auto_populate_reply_metadata により自動で付与される Mentions を Text から削除する
            var postParams = new PostStatusParams(Text: "@foo @bar hoge", inReplyToPost);
            var expected = new CreateTweetParams(Text: "hoge", inReplyToPost)
            {
                AutoPopulateReplyMetadata = true,
                ExcludeReplyUserIds = Array.Empty<PersonId>(),
            };
            Assert.Equal(expected, formatter.CreateParams(postParams));
        }

        [Fact]
        public void CreateParams_RemoveAutoPopuratedMentions_ExcludeTest()
        {
            using var twAccount = new TwitterAccount(AccountKey.New());

            var formatter = new CreateTweetFormatter(twAccount);
            var inReplyToPost = new PostClass
            {
                StatusId = new TwitterStatusId("1"),
                UserId = new TwitterUserId("12345"),
                ScreenName = "foo",
                Text = "@bar tetete",
                ReplyToList = new() { (new TwitterUserId("67890"), "bar") },
            };

            // auto_populate_reply_metadata により自動で付与される Mentions を Text から削除する
            var postParams = new PostStatusParams(Text: "@foo hoge", inReplyToPost);
            var expected = new CreateTweetParams(Text: "hoge", inReplyToPost)
            {
                AutoPopulateReplyMetadata = true,
                ExcludeReplyUserIds = new PersonId[] { new TwitterUserId("67890") },
            };
            Assert.Equal(expected, formatter.CreateParams(postParams));
        }

        [Fact]
        public void CreateParams_RemoveAttachmentUrl_Test()
        {
            using var twAccount = new TwitterAccount(AccountKey.New());
            var formatter = new CreateTweetFormatter(twAccount);

            // 引用ツイートの URL を Text から除去する
            var postParams = new PostStatusParams(Text: "hoge https://twitter.com/twitterapi/status/22634515958");
            var expected = new CreateTweetParams(Text: "hoge")
            {
                AttachmentUrl = "https://twitter.com/twitterapi/status/22634515958",
            };
            Assert.Equal(expected, formatter.CreateParams(postParams));
        }

        [Fact]
        public void CreateParams_RemoveAttachmentUrl_MultipleTest()
        {
            using var twAccount = new TwitterAccount(AccountKey.New());
            var formatter = new CreateTweetFormatter(twAccount);

            // attachment_url は複数指定できないため末尾の URL のみ Text から除去する
            var postParams = new PostStatusParams(Text: "hoge https://twitter.com/muji_net/status/21984934471 https://twitter.com/twitterapi/status/22634515958");
            var expected = new CreateTweetParams(Text: "hoge https://twitter.com/muji_net/status/21984934471")
            {
                AttachmentUrl = "https://twitter.com/twitterapi/status/22634515958",
            };
            Assert.Equal(expected, formatter.CreateParams(postParams));
        }

        [Fact]
        public void CreateParams_RemoveAttachmentUrl_WithMediaTest()
        {
            using var twAccount = new TwitterAccount(AccountKey.New());
            var formatter = new CreateTweetFormatter(twAccount);

            // 引用ツイートと画像添付は併用できないため attachment_url は使用しない（現在は許容されているかも？）
            var postParams = new PostStatusParams(Text: "hoge https://twitter.com/twitterapi/status/22634515958")
            {
                MediaIds = new[] { 1234L },
            };
            var expected = new CreateTweetParams(Text: "hoge https://twitter.com/twitterapi/status/22634515958")
            {
                MediaIds = new[] { 1234L },
            };
            Assert.Equal(expected, formatter.CreateParams(postParams));
        }
    }
}
