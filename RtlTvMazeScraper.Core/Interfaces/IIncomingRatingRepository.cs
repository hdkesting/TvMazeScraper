// <copyright file="IIncomingRatingRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TvMazeScraper.Core.Transfer;

    /// <summary>
    /// Access queued ratings.
    /// </summary>
    public interface IIncomingRatingRepository
    {
        /// <summary>
        /// Gets the queued ratings.
        /// </summary>
        /// <returns>A Task of (possibly empty) list of ratings.</returns>
        Task<List<ShowRating>> GetQueuedRatings();
    }
}
