// <copyright file="IRatingRequestRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Request ratings.
    /// </summary>
    public interface IRatingRequestRepository
    {
        /// <summary>
        /// Requests the rating for the specified IMDB identifier.
        /// </summary>
        /// <param name="imdbId">The imdb identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The rating or <c>null</c>.
        /// </returns>
        Task<decimal?> RequestRating(string imdbId, CancellationToken cancellationToken = default);
    }
}
