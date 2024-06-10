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
using System.Linq;
using OpenTween.Models;

namespace OpenTween.SocialProtocol.Twitter
{
    public record CreateTweetParams(
        string Text,
        PostClass? InReplyTo = null,
        IReadOnlyList<long>? MediaIds = null,
        bool AutoPopulateReplyMetadata = false,
        IReadOnlyList<PersonId>? ExcludeReplyUserIds = null,
        string? AttachmentUrl = null
    )
    {
        public IReadOnlyList<long> MediaIds { get; init; } = MediaIds ?? Array.Empty<long>();

        public IReadOnlyList<PersonId> ExcludeReplyUserIds { get; init; } = ExcludeReplyUserIds ?? Array.Empty<PersonId>();

#pragma warning disable CS8851 // テストコードでしか使用しないため GetHashCode の実装は省略
        public virtual bool Equals(CreateTweetParams? other)
        {
            if (object.ReferenceEquals(this, other))
                return true;

            return other != null &&
                this.Text == other.Text &&
                EqualityComparer<PostClass?>.Default.Equals(this.InReplyTo, other.InReplyTo) &&
                this.MediaIds.SequenceEqual(other.MediaIds) &&
                this.AutoPopulateReplyMetadata == other.AutoPopulateReplyMetadata &&
                this.ExcludeReplyUserIds.SequenceEqual(other.ExcludeReplyUserIds) &&
                this.AttachmentUrl == other.AttachmentUrl;
        }
#pragma warning restore CS8851
    }
}
