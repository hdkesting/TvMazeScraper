// <copyright file="IScraperWorker.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.UI.Workers
{
    /// <summary>
    /// Interface for scraper worker process.
    /// </summary>
    internal interface IScraperWorker
    {
        /// <summary>
        /// Adds the show ids to the queue.
        /// </summary>
        /// <param name="startId">The start identifier.</param>
        /// <param name="count">The count.</param>
        void AddShowIds(int startId, int count = 10);

        /// <summary>
        /// Clears the queue.
        /// </summary>
        void ClearQueue();

        /// <summary>
        /// Does the work.
        /// </summary>
        void DoWork();
    }
}
