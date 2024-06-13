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
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using OpenTween.Connection;
using OpenTween.SocialProtocol.Misskey;

namespace OpenTween.Api.Misskey
{
    public class NoteCreateRequest
    {
        public string? Text { get; set; }

        public string? Visibility { get; set; }

        public MisskeyNoteId? ReplyId { get; set; }

        public MisskeyNoteId? RenoteId { get; set; }

        public MisskeyFileId[] FileIds { get; set; } = Array.Empty<MisskeyFileId>();

        public async Task<MisskeyNote> Send(IApiConnection apiConnection)
        {
            var request = new PostJsonRequest
            {
                RequestUri = new("notes/create", UriKind.Relative),
                JsonString = this.CreateRequestJson(),
            };

            using var response = await apiConnection.SendAsync(request)
                .ConfigureAwait(false);

            var responseBody = await response.ReadAsJson<ResponseBody>()
                .ConfigureAwait(false);

            return responseBody.CreatedNote;
        }

        [DataContract]
        private record RequestBody(
            [property: DataMember(Name = "text", EmitDefaultValue = false)]
            string? Text,
            [property: DataMember(Name = "visibility", EmitDefaultValue = false)]
            string? Visibility,
            [property: DataMember(Name = "replyId", EmitDefaultValue = false)]
            string? ReplyId,
            [property: DataMember(Name = "renoteId", EmitDefaultValue = false)]
            string? RenoteId,
            [property: DataMember(Name = "fileIds", EmitDefaultValue = false)]
            string[]? FileIds
        );

        [DataContract]
        private class ResponseBody
        {
            [DataMember(Name = "createdNote")]
            public MisskeyNote CreatedNote { get; set; } = new();
        }

        private string CreateRequestJson()
        {
            var fileIds = this.FileIds.Length > 0
                ? this.FileIds.Select(x => x.Id).ToArray()
                : null;

            var body = new RequestBody(
                Text: this.Text,
                Visibility: this.Visibility,
                ReplyId: this.ReplyId?.Id,
                RenoteId: this.RenoteId?.Id,
                FileIds: fileIds
            );

            return JsonUtils.SerializeJsonByDataContract(body);
        }
    }
}
