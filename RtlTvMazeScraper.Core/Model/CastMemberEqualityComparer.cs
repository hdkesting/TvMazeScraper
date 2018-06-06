// <copyright file="CastMemberEqualityComparer.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Model
{
    using System.Collections.Generic;
    using RtlTvMazeScraper.Core.Models;

    /// <summary>
    /// Equality comparer for <see cref="CastMember"/>.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{RtlTvMazeScraper.Core.Models.CastMember}" />
    public class CastMemberEqualityComparer : IEqualityComparer<CastMember>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
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
