﻿// <copyright file="ScrapeResult.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Transfer
{
    using System.Net;
    using TvMazeScraper.Core.DTO;

    /// <summary>
    /// The result of a single scrape action.
    /// </summary>
    public class ScrapeResult
    {
        /// <summary>
        /// Gets or sets the show.
        /// </summary>
        /// <value>
        /// The show.
        /// </value>
        public ShowDto Show { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        /// <value>
        /// The HTTP status.
        /// </value>
        public HttpStatusCode HttpStatus { get; set; }

        /// <summary>
        /// Deconstructs this to a show and status tuple.
        /// </summary>
        /// <param name="show">The show.</param>
        /// <param name="status">The status.</param>
        public void Deconstruct(out ShowDto show, out HttpStatusCode status)
        {
            show = this.Show;
            status = this.HttpStatus;
        }
    }
}
