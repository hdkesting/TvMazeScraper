// <copyright file="IIncomingRatingProcessor.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Core.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Processor for incoming ratings.
    /// </summary>
    public interface IIncomingRatingProcessor
    {
        /// <summary>
        /// Processes the incoming ratings.
        /// </summary>
        /// <returns>A Task.</returns>
        Task ProcessIncomingRatings();
    }
}
