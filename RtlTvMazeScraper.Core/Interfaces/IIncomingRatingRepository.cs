// <copyright file="IIncomingRatingRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TvMazeScraper.Core.Transfer;

    /// <summary>
    /// Access queued ratings.
    /// </summary>
    [Obsolete("replace by a call to QueryRating", true)]
    public interface IIncomingRatingRepository
    {
        /// <summary>
        /// Gets the queued ratings.
        /// </summary>
        /// <param name="maxCount">The maximum number of messages to read.</param>
        /// <returns>
        /// A Task of (possibly empty) list of ratings.
        /// </returns>
        Task<List<ShowRating>> GetQueuedRatings(int maxCount);
    }
}
