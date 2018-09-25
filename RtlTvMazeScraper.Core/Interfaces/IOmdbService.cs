// <copyright file="IOmdbService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// The service that calls the Open Movie Database API.
    /// </summary>
    public interface IOmdbService
    {
        /// <summary>
        /// Enriches the show, by getting the IMDb rating.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="imdbId">The IMDb identifier.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        Task EnrichShowWithRating(int showId, string imdbId);
    }
}
