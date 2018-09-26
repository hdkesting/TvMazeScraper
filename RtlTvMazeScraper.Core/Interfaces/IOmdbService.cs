// <copyright file="IOmdbService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Support.Events;

    /// <summary>
    /// The service that calls the Open Movie Database API.
    /// </summary>
    public interface IOmdbService
    {
        /// <summary>
        /// Enriches the show, by getting the IMDb rating.
        /// </summary>
        /// <param name="message">The message detailing the show to enrich.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        Task EnrichShowWithRating(ShowStoredEvent message);
    }
}
