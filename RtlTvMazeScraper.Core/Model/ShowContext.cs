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

        /// <summary>
        /// Further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CastMember>()
                .HasKey(it => new { it.ShowId, it.MemberId });
        }
    }
}
