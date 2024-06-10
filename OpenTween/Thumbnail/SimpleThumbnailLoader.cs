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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween.Thumbnail
{
    public class SimpleThumbnailLoader : IThumbnailLoader
    {
        private readonly string imageUrl;

        public SimpleThumbnailLoader(string imageUrl)
            => this.imageUrl = imageUrl;

        public async Task<MemoryImage> Load(HttpClient http, CancellationToken cancellationToken)
        {
            MemoryImage? image = null;
            try
            {
                using var response = await http.GetAsync(this.imageUrl, cancellationToken)
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                using var imageStream = await response.Content.ReadAsStreamAsync()
                    .ConfigureAwait(false);

                image = await MemoryImage.CopyFromStreamAsync(imageStream)
                    .ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                return image;
            }
            catch (OperationCanceledException)
            {
                image?.Dispose();
                throw;
            }
        }
    }
}
