// <copyright file="ScrapedShow.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.UI.ViewModels
{
    /// <summary>
    /// A scraped show, to report about it.
    /// </summary>
    public class ScrapedShow
    {
        /// <summary>
        /// Gets or sets the show's identifier.
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
        /// Gets or sets the count of cast members.
        /// </summary>
        /// <value>
        /// The cast count.
        /// </value>
        public int CastCount { get; set; }
    }
}
