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

using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween
{
    public class WebpDecoderTest
    {
        [Fact]
        public async Task IsWebpImage_PNGTest()
        {
            using var imgStream = File.OpenRead("Resources/re1.png");
            using var memstream = new MemoryStream();
            await imgStream.CopyToAsync(memstream);
            memstream.TryGetBuffer(out var buffer);

            Assert.False(WebpDecoder.IsWebpImage(buffer));
        }

        [Fact]
        public async Task IsWebpImage_WebPTest()
        {
            using var imgStream = File.OpenRead("Resources/re1.webp");
            using var memstream = new MemoryStream();
            await imgStream.CopyToAsync(memstream);
            memstream.TryGetBuffer(out var buffer);

            Assert.True(WebpDecoder.IsWebpImage(buffer));
        }

#if CI_BUILD
#pragma warning disable xUnit1004
        [Fact(Skip = "WebP画像拡張機能がインストールされている環境でしか動作しない")]
#pragma warning restore xUnit1004
#else
        [Fact]
#endif
        public async Task ConvertFromWebp_SuccessTest()
        {
            using var imgStream = File.OpenRead("Resources/re1.webp");
            using var memstream = new MemoryStream();
            await imgStream.CopyToAsync(memstream);
            memstream.TryGetBuffer(out var buffer);

            var converted = await WebpDecoder.ConvertFromWebp(buffer);
            using var memoryImage = new MemoryImage(converted);
            Assert.Equal(ImageFormat.Png, memoryImage.ImageFormat);
        }

#if CI_BUILD
        [Fact]
#else
#pragma warning disable xUnit1004
        [Fact(Skip = "WebP画像拡張機能がインストールされていない環境に対するテスト")]
#pragma warning restore xUnit1004
#endif
        public async Task ConvertFromWebp_FailTest()
        {
            using var imgStream = File.OpenRead("Resources/re1.webp");
            using var memstream = new MemoryStream();
            await imgStream.CopyToAsync(memstream);
            memstream.TryGetBuffer(out var buffer);

            await Assert.ThrowsAsync<InvalidImageException>(
                () => WebpDecoder.ConvertFromWebp(buffer)
            );
        }
    }
}
