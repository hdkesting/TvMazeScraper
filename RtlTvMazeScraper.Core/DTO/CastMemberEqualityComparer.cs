// <copyright file="CastMemberEqualityComparer.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.DTO
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Equality comparer for <see cref="CastMemberDto"/>.
    /// </summary>
    /// <seealso cref="IEqualityComparer{T}" />
    /// <seealso cref="CastMemberDto" />
    public class CastMemberEqualityComparer : IEqualityComparer<CastMemberDto>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first <see cref="CastMemberDto"/> to compare.</param>
        /// <param name="y">The second <see cref="CastMemberDto"/> to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(CastMemberDto x, CastMemberDto y)
        {
            if (x is null && y is null)
            {
                // both null - I consider that equal
                return true;
            }

            if (x is null || y is null)
            {
                // one is null, the other not - so not equal
                return false;
            }

            return x == y || (x.Id == y.Id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(CastMemberDto obj)
        {
            return obj?.Id ?? throw new ArgumentNullException(nameof(obj));
        }
    }
}
