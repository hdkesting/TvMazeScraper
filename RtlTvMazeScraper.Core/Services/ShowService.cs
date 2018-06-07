// <copyright file="ShowService.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;

    /// <summary>
    /// Service to access shows.
    /// </summary>
    public class ShowService : IShowService
    {
        private readonly IShowRepository showRepository;
        private readonly ILogRepository logRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowService" /> class.
        /// </summary>
        /// <param name="showRepository">The show repository.</param>
        /// <param name="logRepository">The log repository.</param>
        public ShowService(IShowRepository showRepository, ILogRepository logRepository)
        {
            this.showRepository = showRepository;
            this.logRepository = logRepository;
        }

        /// <summary>
        /// Gets the list of shows.
        /// </summary>
        /// <param name="startId">The (show-)id to start at.</param>
        /// <param name="count">The number of shows to download.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<Show>> GetShows(int startId, int count)
        {
            try
            {
                return await this.showRepository.GetShows(startId, count);
            }
            catch (Exception ex)
            {
                this.logRepository.Log(Support.LogLevel.Error, $"Failure to get shows from #{startId}.", ex);
                return new List<Show>();
            }
        }

        /// <summary>
        /// Gets the shows including cast.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <returns>
        /// A list of shows.
        /// </returns>
        public async Task<List<Show>> GetShowsWithCast(int page, int pagesize)
        {
            try
            {
                return await this.showRepository.GetShowsWithCast(page, pagesize);
            }
            catch (Exception ex)
            {
                this.logRepository.Log(Support.LogLevel.Error, $"Failure to get page {page} of shows.", ex);
                return new List<Show>();
            }
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
                this.logRepository.Log(Support.LogLevel.Error, "Failure to get counts.", ex);
                return (-1, -1);
            }
        }

        /// <summary>
        /// Gets the maximum show identifier.
        /// </summary>
        /// <returns>
        /// The highest ID.
        /// </returns>
        public async Task<int> GetMaxShowId()
        {
            try
            {
                return await this.showRepository.GetMaxShowId();
            }
            catch (Exception ex)
            {
                this.logRepository.Log(Support.LogLevel.Error, "Failure to get max id.", ex);
                return -1;
            }
        }

        /// <summary>
        /// Stores the show list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="getCastOfShow">A function to get the cast of one show.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task StoreShowList(List<Show> list, Func<int, Task<List<CastMember>>> getCastOfShow)
        {
            try
            {
                await this.showRepository.StoreShowList(list, getCastOfShow);
            }
            catch (Exception ex)
            {
                this.logRepository.Log(Support.LogLevel.Error, "Failure to store shows.", ex);
            }
        }

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>One Show (if found).</returns>
        public async Task<Show> GetShowById(int id)
        {
            try
            {
                return await this.showRepository.GetShowById(id);
            }
            catch (Exception ex)
            {
                this.logRepository.Log(Support.LogLevel.Error, $"Couldn't get show #{id}.", ex);
                return null;
            }
        }
    }
}
