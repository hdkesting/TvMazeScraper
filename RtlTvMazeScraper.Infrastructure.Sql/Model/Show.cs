// <copyright file="Show.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Sql.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// A show.
    /// </summary>
    public class Show
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
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
        /// Gets or sets date/time this show was last modified.
        /// </summary>
        /// <value>
        /// The last modified date.
        /// </value>
        public DateTimeOffset LastModified { get; set; } = new DateTimeOffset(2018, 10, 1, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// Gets the show's cast.
        /// </summary>
        /// <value>
        /// The cast.
        /// </value>
        public List<ShowCastMember> ShowCastMembers { get; } = new List<ShowCastMember>();
    }
}