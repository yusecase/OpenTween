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
using OpenTween.Api.DataModel;
using OpenTween.Models;
using Xunit;

namespace OpenTween.SocialProtocol.Twitter
{
    public class TwitterV1ClientTest
    {
        [Fact]
        public void GetCursorParams_NoneTest()
        {
            var cursor = (IQueryCursor?)null;
            var (sinceId, maxId) = TwitterV1Client.GetCursorParams(cursor);
            Assert.Null(sinceId);
            Assert.Null(maxId);
        }

        [Fact]
        public void GetCursorParams_TopTest()
        {
            var cursor = new QueryCursor<TwitterStatusId>(CursorType.Top, new("11111"));
            var (sinceId, maxId) = TwitterV1Client.GetCursorParams(cursor);
            Assert.Equal(new("11111"), sinceId);
            Assert.Null(maxId);
        }

        [Fact]
        public void GetCursorParams_BottomTest()
        {
            var cursor = new QueryCursor<TwitterStatusId>(CursorType.Bottom, new("11111"));
            var (sinceId, maxId) = TwitterV1Client.GetCursorParams(cursor);
            Assert.Null(sinceId);
            Assert.Equal(new("11111"), maxId);
        }

        [Fact]
        public void GetCursorFromResponse_EmptyTest()
        {
            var statuses = Array.Empty<TwitterStatus>();
            var (cursorTop, cursorBottom) = TwitterV1Client.GetCursorFromResponse(statuses);
            Assert.Null(cursorTop);
            Assert.Null(cursorBottom);
        }

        [Fact]
        public void GetCursorFromResponse_MinMaxTest()
        {
            var statuses = new[]
            {
                new TwitterStatus
                {
                    IdStr = "11111",
                },
                new TwitterStatus
                {
                    IdStr = "22222",
                },
            };
            var (cursorTop, cursorBottom) = TwitterV1Client.GetCursorFromResponse(statuses);

            var statusIdCursorTop = Assert.IsType<QueryCursor<TwitterStatusId>>(cursorTop);
            Assert.Equal(CursorType.Top, statusIdCursorTop.Type);
            Assert.Equal(new("22222"), statusIdCursorTop.Value);

            var statusIdCursorBottom = Assert.IsType<QueryCursor<TwitterStatusId>>(cursorBottom);
            Assert.Equal(CursorType.Bottom, statusIdCursorBottom.Type);
            Assert.Equal(new("11111"), statusIdCursorBottom.Value);
        }
    }
}
