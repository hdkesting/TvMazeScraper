// <copyright file="ShowService.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Models;

    /// <summary>
    /// Service to access shows.
    /// </summary>
    public class ShowService : IShowService
    {
        private readonly IShowRepository showRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowService"/> class.
        /// </summary>
        /// <param name="showRepository">The show repository.</param>
        public ShowService(IShowRepository showRepository)
        {
            this.showRepository = showRepository;
        }

        /// <summary>
        /// Gets the list of shows.
        /// </summary>
        /// <param name="startId">The id to start at.</param>
        /// <param name="count">The number of shows to download.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public Task<List<Show>> GetShows(int startId, int count)
        {
            return this.showRepository.GetShows(startId, count);
        }

        /// <summary>
        /// Gets the shows including cast.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public Task<List<Show>> GetShowsWithCast(int page, int pagesize)
        {
            return this.GetShowsWithCast(page, pagesize);
        }

        /// <summary>
        /// Gets the counts of shows and cast.
        /// </summary>
        /// <returns>
        /// A tuple having counts of shows and castmembers.
        /// </returns>
        public async Task<(int shows, int members)> GetCounts()
        {
            try
            {
                return await this.showRepository.GetCounts();
            }
            catch (Exception ex)
            {
                // TODO log exception
                System.Diagnostics.Debug.WriteLine(ex);
                return (-1, -1);
            }
        }

        /// <summary>
        /// Gets the maximum show identifier.
        /// </summary>
        /// <returns>
        /// The highest ID.
        /// </returns>
        public Task<int> GetMaxShowId()
        {
            return this.showRepository.GetMaxShowId();
        }

        /// <summary>
        /// Stores the show list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="getCastOfShow">A function to get the cast of one show.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task StoreShowList(List<Show> list, Func<int, Task<List<CastMember>>> getCastOfShow)
        {
            return this.showRepository.StoreShowList(list, getCastOfShow);
        }
    }
}
