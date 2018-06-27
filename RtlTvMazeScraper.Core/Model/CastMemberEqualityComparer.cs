// <copyright file="CastMemberEqualityComparer.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Equality comparer for <see cref="CastMember"/>.
    /// </summary>
    /// <seealso cref="IEqualityComparer{T}" />
    /// <seealso cref="CastMember" />
    public class CastMemberEqualityComparer : IEqualityComparer<CastMember>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first <see cref="CastMember"/> to compare.</param>
        /// <param name="y">The second <see cref="CastMember"/> to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(CastMember x, CastMember y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.MemberId == y.MemberId;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(CastMember obj)
        {
            return (obj.ShowId * 1001) + obj.MemberId;
        }
    }
}
