// <copyright file="IScraperWorker.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Workers
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for scraper worker process.
    /// </summary>
    internal interface IScraperWorker
    {
        /// <summary>
        /// Does the work.
        /// </summary>
        /// <returns>A Task.</returns>
        Task<WorkResult> DoWork();
    }
}
