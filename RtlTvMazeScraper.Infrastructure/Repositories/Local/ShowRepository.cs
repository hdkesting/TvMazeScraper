// <copyright file="ShowRepository.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Repositories.Local
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;

    /// <summary>
    /// A repository for locally stored shows.
    /// </summary>
    /// <seealso cref="RtlTvMazeScraper.Interfaces.IShowRepository" />
    public class ShowRepository : IShowRepository
    {
        private readonly string connstr;
        private readonly Data.ShowContext showContext;
        private readonly ILogRepository logRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowRepository" /> class.
        /// </summary>
        /// <param name="settingRepository">The setting repository.</param>
        /// <param name="logRepository">The log repository.</param>
        public ShowRepository(ISettingRepository settingRepository, ILogRepository logRepository)
        {
            this.connstr = settingRepository.ConnectionString;
            this.showContext = new Data.ShowContext();
            this.logRepository = logRepository;
        }

        /// <summary>
        /// Gets the list of shows (including cast).
        /// </summary>
        /// <param name="startId">The id to start at.</param>
        /// <param name="count">The number of shows to download.</param>
        /// <returns>
        /// A list of shows with cast.
        /// </returns>
        public async Task<List<Show>> GetShows(int startId, int count)
        {
            return await this.showContext.Shows
                .Include(s => s.CastMembers)
                .Where(s => s.Id >= startId)
                .OrderBy(s => s.Id)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the shows including cast.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <returns>
        /// A list of shows with cast.
        /// </returns>
        public async Task<List<Show>> GetShowsWithCast(int page, int pagesize)
        {
            return await this.showContext.Shows
                .Include(s => s.CastMembers)
                .OrderBy(s => s.Id)
                .Skip(page * pagesize)
                .Take(pagesize)
                .ToListAsync();
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
            var memberEqualityComparer = new CastMemberEqualityComparer();

            foreach (var show in list)
            {
                try
                {
                    if (!show.CastMembers.Any() && getCastOfShow != null)
                    {
                        show.CastMembers.AddRange(await getCastOfShow(show.Id));
                    }

                    // there are duplicate "persons" in the cast (when they have different roles)
                    var realcast = show.CastMembers.Distinct(memberEqualityComparer).ToList();
                    if (realcast.Count < show.CastMembers.Count)
                    {
                        show.CastMembers.Clear();
                        show.CastMembers.AddRange(realcast);
                    }

                    var existing = await this.GetShowById(show.Id);

                    if (existing == null)
                    {
                        await this.AddShow(show);
                    }
                    else
                    {
                        await this.UpdateShow(show, existing);
                    }
                }
                catch (Exception ex)
                {
                    this.logRepository.Log(Core.Support.LogLevel.Error, $"Error storing show #{show.Id}.", ex);
                }
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
            // alas, EF doesn't support running these in parallel.
            var numberOfShows = await this.showContext.Shows.CountAsync();
            var numberOfMembers = await this.showContext.CastMembers.CountAsync();

            return (numberOfShows, numberOfMembers);
        }

        /// <summary>
        /// Gets the maximum show identifier.
        /// </summary>
        /// <returns>
        /// The highest ID.
        /// </returns>
        public Task<int> GetMaxShowId()
        {
            return this.showContext.Shows
                .Select(s => s.Id)
                .MaxAsync();
        }

        /// <summary>
        /// Gets the cast of show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>A list of cast members.</returns>
        public Task<List<CastMember>> GetCastOfShow(int showId)
        {
            return this.showContext.CastMembers
                .Where(cm => cm.ShowId == showId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>One Show (if found).</returns>
        public Task<Show> GetShowById(int id)
        {
            return this.showContext.Shows
                .Include(s => s.CastMembers)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        private async Task AddShow(Show show)
        {
            this.showContext.Shows.Add(show);
            await this.showContext.SaveChangesAsync();
        }

        private async Task UpdateShow(Show newShow, Show storedShow)
        {
            storedShow.Name = newShow.Name;

            foreach (var newMember in newShow.CastMembers)
            {
                var storedMember = storedShow.CastMembers.FirstOrDefault(m => m.MemberId == newMember.MemberId);

                if (storedMember == null)
                {
                    storedShow.CastMembers.Add(newMember);
                }
                else
                {
                    storedMember.Name = newMember.Name;
                    storedMember.Birthdate = newMember.Birthdate;
                }
            }

            storedShow.CastMembers.RemoveAll(m => !newShow.CastMembers.Any(m2 => m2.MemberId == m.MemberId));

            await this.showContext.SaveChangesAsync();
        }
    }
}