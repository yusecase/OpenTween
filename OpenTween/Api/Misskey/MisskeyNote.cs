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

namespace OpenTween.Api.Misskey
{
    [DataContract]
    public class MisskeyNote
    {
        [DataMember(Name = "id")]
        public string Id { get; set; } = "";

        [DataMember(Name = "createdAt")]
        public string CreatedAt { get; set; } = "";

        [DataMember(Name = "text")]
        public string? Text { get; set; }

        [DataMember(Name = "userId")]
        public string UserId { get; set; } = "";

        [DataMember(Name = "user")]
        public MisskeyUserLite User { get; set; } = new();

        [DataMember(Name = "replyId", IsRequired = false)]
        public string? ReplyId { get; set; }

        [DataMember(Name = "reply", IsRequired = false)]
        public MisskeyNote? Reply { get; set; }

        [DataMember(Name = "renoteId", IsRequired = false)]
        public string? RenoteId { get; set; }

        [DataMember(Name = "renote", IsRequired = false)]
        public MisskeyNote? Renote { get; set; }

        [DataMember(Name = "visibility")]
        public string Visibility { get; set; } = "";

        [DataMember(Name = "mentions", IsRequired = false)]
        public string[] Mentions { get; set; } = Array.Empty<string>();

        [DataMember(Name = "files")]
        public MisskeyDriveFile[] Files { get; set; } = Array.Empty<MisskeyDriveFile>();

        [DataMember(Name = "myReaction", IsRequired = false)]
        public string? MyReaction { get; set; }
    }
}
