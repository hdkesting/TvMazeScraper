// <copyright file="ShowWithCast.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Mongo.Model
{
    using System.Collections.Generic;
    using RtlTvMazeScraper.Core.DTO;

    /// <summary>
    /// The show with plain list of cast members.
    /// </summary>
    /// <remarks>
    /// Todo: make this the default and the current Show the db-specific exception.
    /// </remarks>
    internal class ShowWithCast
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

#pragma warning disable CA2227 // Collection properties should be read only - MongoDB needs to be able to write to it.

        /// <summary>
        /// Gets or sets all cast members.
        /// </summary>
        /// <remarks>
        /// Apparently MongoDB needs a <b>writable</b> list property.
        /// </remarks>
        /// <value>
        /// The cast.
        /// </value>
        public List<CastMemberDto> Cast { get; set; } = new List<CastMemberDto>();
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
