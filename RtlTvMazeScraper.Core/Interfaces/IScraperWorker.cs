// <copyright file="IScraperWorker.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Interfaces
{
    using System.Threading.Tasks;
    using TvMazeScraper.Core.Support;

    /// <summary>
    /// Interface for scraper worker process.
    /// </summary>
    public interface IScraperWorker
    {
        /// <summary>
        /// Does the work.
        /// </summary>
        /// <returns>A Task.</returns>
        Task<WorkResult> DoWorkOnSingleShow();

        /// <summary>
        /// Does the work by reading many shows.
        /// </summary>
        /// <returns>A Task.</returns>
        Task<WorkResult> DoWorkOnManyShows();
    }
}
