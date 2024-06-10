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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace OpenTween
{
    public class WebpDecoder
    {
        // reference: https://mimesniff.spec.whatwg.org/#matching-an-image-type-pattern
        private static readonly byte[] PatternMask =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        };

        private static readonly byte[] BytePattern =
        {
            0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00,
            0x57, 0x45, 0x42, 0x50, 0x56, 0x50,
        };

        public static bool IsWebpImage(ArraySegment<byte> buffer)
        {
            return buffer.WithIndex().Take(PatternMask.Length)
                .All(x => (x.Value & PatternMask[x.Index]) == BytePattern[x.Index]);
        }

        public static async Task<ArraySegment<byte>> ConvertFromWebp(ArraySegment<byte> inputBuffer)
        {
            const uint WINCODEC_ERR_COMPONENTINITIALIZEFAILURE = 0x88982F8B;

            try
            {
                using var inputStream = new MemoryStream(inputBuffer.Array, inputBuffer.Offset, inputBuffer.Count, writable: false, publiclyVisible: true);
                using var raInStream = inputStream.AsRandomAccessStream();
                var decoder = await BitmapDecoder.CreateAsync(raInStream);
                using var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                using var outputStream = new MemoryStream();
                using var raOutStream = outputStream.AsRandomAccessStream();

                var encoderId = BitmapEncoder.PngEncoderId;
                var encoder = await BitmapEncoder.CreateAsync(encoderId, raOutStream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                await encoder.FlushAsync();

                var ret = outputStream.TryGetBuffer(out var outBuffer);
                Debug.Assert(ret, "TryGetBuffer() == true");

                return outBuffer;
            }
            catch (Exception ex)
                when (unchecked((uint)ex.HResult) == WINCODEC_ERR_COMPONENTINITIALIZEFAILURE)
            {
                // 「WebP画像拡張機能」がインストールされていない環境ではエラーになる
                throw new InvalidImageException($"WebP codec initialization error (HRESULT: 0x{WINCODEC_ERR_COMPONENTINITIALIZEFAILURE:X})", ex);
            }
        }
    }
}
