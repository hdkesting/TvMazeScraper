// <copyright file="IShowContext.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using RtlTvMazeScraper.Core.Model;

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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Number of changes.
        /// </returns>
        Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
    }
}
