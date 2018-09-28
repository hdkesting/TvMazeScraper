// <copyright file="ShowForJson.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI.ViewModels
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

#pragma warning disable CA2227 // Collection properties should be read only
        /// <summary>
        /// Gets or sets the show's cast.
        /// </summary>
        /// <value>
        /// The cast.
        /// </value>
        public List<CastMemberForJson> Cast { get; set; } = new List<CastMemberForJson>();
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
