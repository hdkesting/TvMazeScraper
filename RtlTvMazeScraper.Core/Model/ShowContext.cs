// <copyright file="ShowContext.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Model
{
    using Microsoft.EntityFrameworkCore;
    using RtlTvMazeScraper.Core.Interfaces;

    /// <summary>
    /// The DB context for the show database.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    public class ShowContext : DbContext, IShowContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public ShowContext(DbContextOptions<ShowContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the shows.
        /// </summary>
        /// <value>
        /// The shows.
        /// </value>
        public DbSet<Show> Shows { get; set; }

        /// <summary>
        /// Gets or sets the cast members.
        /// </summary>
        /// <value>
        /// The cast members.
        /// </value>
        public DbSet<CastMember> CastMembers { get; set; }
    }
}
