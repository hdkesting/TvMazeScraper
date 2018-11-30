// <copyright file="IRatingService.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Service for finding ratings.
    /// </summary>
    public interface IRatingService
    {
        /// <summary>
        /// Processes a batch of ratings.
        /// </summary>
        /// <param name="count">The number of ratings to try.</param>
        /// <returns>A Task.</returns>
        Task ProcessSomeRatings(int count);
    }
}
