// <copyright file="IShowContext.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Interfaces
{
    using System.Data.Entity;
    using System.Threading.Tasks;
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
        /// <returns>Number of changes.</returns>
        Task<int> SaveChangesAsync();
    }
}
