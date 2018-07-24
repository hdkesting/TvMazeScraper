// <copyright file="ShowCastMember.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Model
{
    /// <summary>
    /// Represents the many-to-many relationship between <see cref="Show"/>s and <see cref="CastMember"/>s.
    /// </summary>
    public class ShowCastMember
    {
        /// <summary>
        /// Gets or sets the show identifier (FK).
        /// </summary>
        /// <value>
        /// The show identifier.
        /// </value>
        public int ShowId { get; set; }

        /// <summary>
        /// Gets or sets the show.
        /// </summary>
        /// <value>
        /// The show.
        /// </value>
        public Show Show { get; set; }

        /// <summary>
        /// Gets or sets the cast member identifier (FK).
        /// </summary>
        /// <value>
        /// The cast member identifier.
        /// </value>
        public int CastMemberId { get; set; }

        /// <summary>
        /// Gets or sets the cast member.
        /// </summary>
        /// <value>
        /// The cast member.
        /// </value>
        public CastMember CastMember { get; set; }
    }
}
