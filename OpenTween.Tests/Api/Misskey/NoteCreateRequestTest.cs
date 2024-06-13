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
using System.Threading.Tasks;
using Moq;
using OpenTween.Connection;
using OpenTween.SocialProtocol.Misskey;
using Xunit;

namespace OpenTween.Api.Misskey
{
    public class NoteCreateRequestTest
    {
        [Fact]
        public async Task Send_Test()
        {
            var response = TestUtils.CreateApiResponse(new MisskeyNote());

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.SendAsync(It.IsAny<IHttpRequest>())
                )
                .Callback<IHttpRequest>(x =>
                {
                    var request = Assert.IsType<PostJsonRequest>(x);
                    Assert.Equal(new("notes/create", UriKind.Relative), request.RequestUri);
                    Assert.Equal(
                        """{"fileIds":["bbbbb"],"replyId":"aaaaa","text":"tetete","visibility":"public"}""",
                        request.JsonString
                    );
                })
                .ReturnsAsync(response);

            var request = new NoteCreateRequest
            {
                Text = "tetete",
                Visibility = "public",
                ReplyId = new("aaaaa"),
                FileIds = new[] { new MisskeyFileId("bbbbb") },
            };
            await request.Send(mock.Object);

            mock.VerifyAll();
        }

        [Fact]
        public async Task Send_RenoteTest()
        {
            var response = TestUtils.CreateApiResponse(new MisskeyNote());

            var mock = new Mock<IApiConnection>();
            mock.Setup(x =>
                    x.SendAsync(It.IsAny<IHttpRequest>())
                )
                .Callback<IHttpRequest>(x =>
                {
                    var request = Assert.IsType<PostJsonRequest>(x);
                    Assert.Equal(new("notes/create", UriKind.Relative), request.RequestUri);
                    Assert.Equal(
                        """{"renoteId":"aaaaa"}""",
                        request.JsonString
                    );
                })
                .ReturnsAsync(response);

            var request = new NoteCreateRequest
            {
                RenoteId = new("aaaaa"),
            };
            await request.Send(mock.Object);

            mock.VerifyAll();
        }
    }
}
