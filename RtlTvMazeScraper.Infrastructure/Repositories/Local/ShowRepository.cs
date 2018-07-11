// <copyright file="ShowRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Repositories.Local
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;
    using RtlTvMazeScraper.Core.Transfer;

    /// <summary>
    /// A repository for locally stored shows.
    /// </summary>
    /// <seealso cref="IShowRepository" />
    [CLSCompliant(false)]
    public class ShowRepository : IShowRepository
    {
        private readonly ILogger<ShowRepository> logger;
        private readonly IShowContext showContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowRepository" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="showContext">The show context.</param>
        public ShowRepository(
            ILogger<ShowRepository> logger,
            IShowContext showContext)
        {
            this.logger = logger;
            this.showContext = showContext;
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
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the shows including cast.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of shows with cast.
        /// </returns>
        public async Task<List<Show>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken)
        {
            return await this.showContext.Shows
                .Include(s => s.CastMembers)
                .OrderBy(s => s.Id)
                .Skip(page * pagesize)
                .Take(pagesize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
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
                        var cast = await getCastOfShow(show.Id).ConfigureAwait(false);
                        show.CastMembers.AddRange(cast);
                    }

                    // there are duplicate "persons" in the cast (when they have different roles) - we are only interested in persons, not roles
                    var realcast = show.CastMembers.Distinct(memberEqualityComparer).ToList();
                    if (realcast.Count < show.CastMembers.Count)
                    {
                        show.CastMembers.Clear();
                        show.CastMembers.AddRange(realcast);
                    }

                    var existing = await this.GetShowById(show.Id).ConfigureAwait(false);

                    if (existing == null)
                    {
                        await this.AddShow(show).ConfigureAwait(false);
                    }
                    else
                    {
                        await this.UpdateShow(show, existing).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error storing show #{showId}.", show.Id);
                }
            }
        }

        /// <summary>
        /// Gets the counts of shows and cast.
        /// </summary>
        /// <returns>
        /// A tuple having counts of shows and castmembers.
        /// </returns>
        public async Task<StorageCount> GetCounts()
        {
            // EFCore *can* run this in parallel!
            var showTask = this.showContext.Shows.CountAsync();
            var castTask = this.showContext.CastMembers.CountAsync();

            await Task.WhenAll(showTask, castTask).ConfigureAwait(false);

            var numberOfShows = showTask.Result;
            var numberOfMembers = castTask.Result;

            return new StorageCount(numberOfShows, numberOfMembers);
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

        private Task AddShow(Show show)
        {
            this.showContext.Shows.Add(show);
            return this.showContext.SaveChangesAsync();
        }

        private Task UpdateShow(Show newShow, Show storedShow)
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

            return this.showContext.SaveChangesAsync();
        }
    }
}