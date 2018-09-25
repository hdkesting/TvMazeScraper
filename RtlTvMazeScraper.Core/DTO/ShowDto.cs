// <copyright file="ShowDto.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.DTO
{
    using System.Collections.Generic;

    /// <summary>
    /// A show.
    /// </summary>
    public sealed class ShowDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the show.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the IMDb identifier.
        /// </summary>
        /// <remarks>A string like "tt0898266".</remarks>
        /// <value>
        /// The IMDb identifier.
        /// </value>
        public string ImdbId { get; set; }

        /// <summary>
        /// Gets or sets the IMDb rating.
        /// </summary>
        /// <value>
        /// The IMDb rating, a value between 1.0 and 10.0.
        /// </value>
        public decimal? ImdbRating { get; set; }

        /// <summary>
        /// Gets the show's cast.
        /// </summary>
        /// <value>
        /// The cast.
        /// </value>
        public List<CastMemberDto> CastMembers { get; } = new List<CastMemberDto>();

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(ShowDto)} '{this.Name}' ({this.Id}/{this.ImdbId})";
        }
    }
}