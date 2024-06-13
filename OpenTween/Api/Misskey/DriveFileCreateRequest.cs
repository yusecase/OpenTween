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
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTween.Connection;

namespace OpenTween.Api.Misskey
{
    public class DriveFileCreateRequest
    {
        public required IMediaItem File { get; set; }

        public string? Comment { get; set; }

        public async Task<MisskeyDriveFile> Send(IApiConnection apiConnection)
        {
            apiConnection.ThrowIfUnauthorizedScope("write:drive");

            var request = new PostMultipartRequest
            {
                RequestUri = new("drive/files/create", UriKind.Relative),
                Query = this.CreateQuery(),
                Media = new Dictionary<string, IMediaItem>
                {
                    ["file"] = this.File,
                },
            };

            using var response = await apiConnection.SendAsync(request)
                .ConfigureAwait(false);

            var responseBody = await response.ReadAsJson<MisskeyDriveFile>()
                .ConfigureAwait(false);

            return responseBody;
        }

        private IDictionary<string, string> CreateQuery()
        {
            var query = new Dictionary<string, string>();

            if (!MyCommon.IsNullOrEmpty(this.Comment))
                query["comment"] = this.Comment;

            return query;
        }
    }
}
