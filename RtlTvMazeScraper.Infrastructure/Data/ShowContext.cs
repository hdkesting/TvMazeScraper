// <copyright file="ShowContext.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Data
{
    using System.Data.Entity;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;

    /// <summary>
    /// The DB context for the show database.
    /// </summary>
    /// <seealso cref="System.Data.Entity.DbContext" />
    public class ShowContext : DbContext, IShowContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowContext"/> class and specifies a connection string to use.
        /// </summary>
        public ShowContext()
            : base("ShowContext")
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
