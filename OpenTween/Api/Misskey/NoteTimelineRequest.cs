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
using System.Runtime.Serialization;
using System.Threading.Tasks;
using OpenTween.Connection;
using OpenTween.SocialProtocol.Misskey;

namespace OpenTween.Api.Misskey
{
    public class NoteTimelineRequest
    {
        public int? Limit { get; set; }

        public MisskeyNoteId? SinceId { get; set; }

        public MisskeyNoteId? UntilId { get; set; }

        public async Task<MisskeyNote[]> Send(IApiConnection apiConnection)
        {
            var request = new PostJsonRequest
            {
                RequestUri = new("notes/timeline", UriKind.Relative),
                JsonString = this.CreateRequestJson(),
            };

            using var response = await apiConnection.SendAsync(request)
                .ConfigureAwait(false);

            return await response.ReadAsJson<MisskeyNote[]>()
                .ConfigureAwait(false);
        }

        [DataContract]
        private record RequestBody(
            [property: DataMember(Name = "limit", EmitDefaultValue = false)]
            int? Limit,
            [property: DataMember(Name = "sinceId", EmitDefaultValue = false)]
            string? SinceId,
            [property: DataMember(Name = "untilId", EmitDefaultValue = false)]
            string? UntilId
        );

        private string CreateRequestJson()
        {
            var body = new RequestBody(
                Limit: this.Limit,
                SinceId: this.SinceId?.Id,
                UntilId: this.UntilId?.Id
            );

            return JsonUtils.SerializeJsonByDataContract(body);
        }
    }
}
