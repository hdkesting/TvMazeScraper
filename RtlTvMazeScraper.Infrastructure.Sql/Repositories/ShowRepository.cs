// <copyright file="ShowRepository.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Sql.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TvMazeScraper.Core.DTO;
    using TvMazeScraper.Core.Interfaces;
    using TvMazeScraper.Core.Transfer;
    using TvMazeScraper.Infrastructure.Sql.Interfaces;
    using TvMazeScraper.Infrastructure.Sql.Model;

    /// <summary>
    /// A repository for locally stored shows.
    /// </summary>
    /// <seealso cref="IShowRepository" />
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
        public async Task<List<ShowDto>> GetShows(int startId, int count)
        {
            var shows = await this.showContext.Shows
                .Include(s => s.ShowCastMembers)
                    .ThenInclude(scm => scm.CastMember)
                .Where(s => s.Id >= startId)
                .OrderBy(s => s.Id)
                .Take(count)
                .ToListAsync()
                .ConfigureAwait(false);

            return shows.Select(this.ConvertShow).ToList();
        }

        /// <summary>
        /// Gets a page of the shows including cast.
        /// </summary>
        /// <param name="page">The page number (0-based).</param>
        /// <param name="pagesize">The size of the page.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>
        /// A list of shows with cast.
        /// </returns>
        public async Task<List<ShowDto>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken)
        {
            var shows = await this.showContext.Shows
                .Include(s => s.ShowCastMembers)
                    .ThenInclude(scm => scm.CastMember)
                .OrderBy(s => s.Id)
                .Skip(page * pagesize)
                .Take(pagesize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return shows.Select(this.ConvertShow).ToList();
        }

        /// <summary>
        /// Stores the show list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="getCastOfShow">A function to get the cast of one show.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task StoreShowList(List<ShowDto> list, Func<int, Task<List<CastMemberDto>>> getCastOfShow)
        {
            var memberEqualityComparer = new CastMemberEqualityComparer();

            foreach (var show in list)
            {
                try
                {
                    if (!show.CastMembers.Any() && !(getCastOfShow is null))
                    {
                        var cast = await getCastOfShow(show.Id).ConfigureAwait(false);
                        show.CastMembers.AddRange(cast);
                    }

                    // there are duplicate actors in the cast (when they have different roles) - we are only interested in persons, not roles
                    var realcast = show.CastMembers.Distinct(memberEqualityComparer).ToList();

                    var ids = realcast.Select(c => c.Id).ToList();
                    var storedcast = this.showContext.CastMembers.Where(m => ids.Contains(m.Id)).ToList();

                    show.CastMembers.Clear();
                    foreach (var member in realcast)
                    {
                        var storedmember = storedcast.FirstOrDefault(c => c.Id == member.Id);
                        if (storedmember is null)
                        {
                            // not stored yet
                            show.CastMembers.Add(member);
                        }
                        else
                        {
                            show.CastMembers.Add(new CastMemberDto { Id = storedmember.Id, Name = storedmember.Name, Birthdate = storedmember.Birthdate });
                        }
                    }

                    var localshow = this.ConvertShow(show);
                    var existingShow = await this.GetLocalShowById(show.Id).ConfigureAwait(false);

                    if (existingShow is null)
                    {
                        await this.AddShow(localshow).ConfigureAwait(false);
                    }
                    else
                    {
                        await this.UpdateShow(localshow, existingShow).ConfigureAwait(false);
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
        public async Task<List<CastMemberDto>> GetCastOfShow(int showId)
        {
            var show = await this.showContext.Shows
                .Include(s => s.ShowCastMembers)
                    .ThenInclude(scm => scm.CastMember)
                .SingleOrDefaultAsync(s => s.Id == showId)
                .ConfigureAwait(false);

            return show?.ShowCastMembers
                .Select(it => it.CastMember)
                .Select(it => new CastMemberDto
                {
                    Id = it.Id,
                    Name = it.Name,
                    Birthdate = it.Birthdate,
                })
                .ToList();
        }

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>One Show (if found).</returns>
        public async Task<ShowDto> GetShowById(int id)
        {
            var localshow = await this.showContext.Shows
                .Include(s => s.ShowCastMembers)
                .ThenInclude(scm => scm.CastMember)
                .FirstOrDefaultAsync(s => s.Id == id)
                .ConfigureAwait(false);

            return this.ConvertShow(localshow);
        }

        /// <summary>
        /// Sets the rating of the specified show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="rating">The rating.</param>
        /// <returns>
        /// A <see cref="Task" />.
        /// </returns>
        public async Task SetRating(int showId, decimal rating)
        {
            var show = await this.showContext.Shows.SingleOrDefaultAsync(s => s.Id == showId).ConfigureAwait(false);

            if (show != null)
            {
                show.ImdbRating = rating;
                await this.showContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private Task<Show> GetLocalShowById(int id)
        {
            return this.showContext.Shows
                .Include(s => s.ShowCastMembers)
                .ThenInclude(scm => scm.CastMember)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        private ShowDto ConvertShow(Show localshow)
        {
            // TODO use mapper
            var coreshow = new ShowDto { Id = localshow.Id, Name = localshow.Name };
            coreshow.CastMembers.AddRange(localshow.ShowCastMembers
                .Select(scm => scm.CastMember)
                .Select(cm => new CastMemberDto { Id = cm.Id, Name = cm.Name, Birthdate = cm.Birthdate }));
            return coreshow;
        }

        private Show ConvertShow(ShowDto coreshow)
        {
            var modelshow = new Show { Id = coreshow.Id, Name = coreshow.Name };
            modelshow.ShowCastMembers.AddRange(coreshow.CastMembers
                .Select(cm => new ShowCastMember
                {
                    Show = modelshow,
                    CastMember = new CastMember
                    {
                        Id = cm.Id,
                        Name = cm.Name,
                        Birthdate = cm.Birthdate,
                    },
                }));
            return modelshow;
        }

        private Task AddShow(Show show)
        {
            this.showContext.Shows.Add(show);
            return this.showContext.SaveChangesAsync();
        }

        private async Task UpdateShow(Show newShow, Show storedShow)
        {
            storedShow.Name = newShow.Name;

            foreach (var newMember in newShow.ShowCastMembers.Select(scm => scm.CastMember))
            {
                var storedMember = storedShow.ShowCastMembers.FirstOrDefault(m => m.CastMemberId == newMember.Id);

                if (storedMember is null)
                {
                    var storedActor = await this.showContext.CastMembers.SingleOrDefaultAsync(m => m.Id == newMember.Id).ConfigureAwait(false);

                    if (storedActor is null)
                    {
                        // also not stored for another show
                        storedShow.ShowCastMembers.Add(new ShowCastMember { Show = storedShow, CastMember = newMember });
                    }
                    else
                    {
                        storedShow.ShowCastMembers.Add(new ShowCastMember { Show = storedShow, CastMember = storedActor });
                    }
                }
                else
                {
                    storedMember.CastMember.Name = newMember.Name;
                    storedMember.CastMember.Birthdate = newMember.Birthdate;
                }
            }

            // remove the relation, not the actors
            storedShow.ShowCastMembers.RemoveAll(m => !newShow.ShowCastMembers.Any(m2 => m2.CastMemberId == m.CastMemberId));

            await this.showContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}