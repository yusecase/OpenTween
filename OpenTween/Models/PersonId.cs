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
using System.Diagnostics;

namespace OpenTween.Models
{
    [DebuggerDisplay("{IdType}:{Id}")]
    public abstract class PersonId
        : IEquatable<PersonId>, IComparable<PersonId>
    {
        public abstract string IdType { get; }

        public abstract string Id { get; }

        public virtual int CompareTo(PersonId other)
        {
            var compareByIdType = this.IdType.CompareTo(other.IdType);
            if (compareByIdType != 0)
                return compareByIdType;

            // 辞書順による比較のみだと "20" > "100" となってしまうため文字数による比較も加える
            var compareByIdLength = this.Id.Length.CompareTo(other.Id.Length);
            if (compareByIdLength != 0)
                return compareByIdLength;

            return this.Id.CompareTo(other.Id);
        }

        public virtual bool Equals(PersonId other)
            => this.IdType == other.IdType && this.Id == other.Id;

        public override bool Equals(object obj)
            => obj is PersonId otherId && this.Equals(otherId);

        public override int GetHashCode()
            => this.IdType.GetHashCode() ^ this.Id.GetHashCode();

        public override string ToString()
            => this.Id;

        public static bool operator ==(PersonId? left, PersonId? right)
            => EqualityComparer<PersonId?>.Default.Equals(left, right);

        public static bool operator !=(PersonId? left, PersonId? right)
            => !EqualityComparer<PersonId?>.Default.Equals(left, right);

        public static bool operator <(PersonId left, PersonId right)
            => Comparer<PersonId>.Default.Compare(left, right) < 0;

        public static bool operator <=(PersonId left, PersonId right)
            => Comparer<PersonId>.Default.Compare(left, right) <= 0;

        public static bool operator >=(PersonId left, PersonId right)
            => Comparer<PersonId>.Default.Compare(left, right) >= 0;

        public static bool operator >(PersonId left, PersonId right)
            => Comparer<PersonId>.Default.Compare(left, right) > 0;
    }
}
