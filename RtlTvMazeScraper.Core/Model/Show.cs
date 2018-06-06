// <copyright file="Show.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Models
{
    using System.Collections.Generic;

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
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the show's cast.
        /// </summary>
        /// <value>
        /// The cast.
        /// </value>
        public virtual List<CastMember> Cast { get; } = new List<CastMember>();
    }
}