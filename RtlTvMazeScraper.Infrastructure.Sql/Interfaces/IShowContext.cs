// <copyright file="IShowContext.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Sql.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TvMazeScraper.Infrastructure.Sql.Model;

    /// <summary>
    /// Interface for DbContext of shows.
    /// </summary>
    public interface IShowContext
    {
        /// <summary>
        /// Gets the shows.
        /// </summary>
        /// <value>
        /// The shows.
        /// </value>
        DbSet<Show> Shows { get; }

        /// <summary>
        /// Gets the cast members.
        /// </summary>
        /// <value>
        /// The cast members.
        /// </value>
        DbSet<CastMember> CastMembers { get; }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns>Number of changes.</returns>
        int SaveChanges();

        /// <summary>
        /// Saves the changes asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation. Default <see cref="CancellationToken.None"/></param>
        /// <returns>
        /// Number of changes.
        /// </returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
