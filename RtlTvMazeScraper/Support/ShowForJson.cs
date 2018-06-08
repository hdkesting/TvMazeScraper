// <copyright file="ShowForJson.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Support
{
    using System.Collections.Generic;

    /// <summary>
    /// Show details, for JSON serialisation.
    /// </summary>
    public class ShowForJson
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
        /// Gets or sets the show's cast.
        /// </summary>
        /// <value>
        /// The cast.
        /// </value>
        public List<CastMemberForJson> Cast { get; set; } = new List<CastMemberForJson>();
    }
}