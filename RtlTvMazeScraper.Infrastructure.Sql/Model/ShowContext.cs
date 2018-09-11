// <copyright file="ShowContext.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Sql.Model
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using RtlTvMazeScraper.Infrastructure.Sql.Interfaces;

    /// <summary>
    /// The DB context for the show database.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
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
        /// Gets or sets the show cast members, the many-to-many relation between <see cref="Show"/> and <see cref="CastMember"/>.
        /// </summary>
        /// <value>
        /// The show cast members.
        /// </value>
        public DbSet<ShowCastMember> ShowCastMembers { get; set; }

        /// <summary>
        /// Further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="Microsoft.EntityFrameworkCore.DbSet{T}" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>
        /// If a model is explicitly set on the options for this context (via <see cref="Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // many-to-many link table has a double key
            modelBuilder.Entity<ShowCastMember>()
                .HasKey(it => new { it.ShowId, it.CastMemberId });

            // setup two-sided relation. https://docs.microsoft.com/en-us/ef/core/modeling/relationships#many-to-many
            modelBuilder.Entity<ShowCastMember>()
                .HasOne(it => it.Show)
                .WithMany(it => it.ShowCastMembers)
                .HasForeignKey(it => it.ShowId);

            modelBuilder.Entity<ShowCastMember>()
                .HasOne(it => it.CastMember)
                .WithMany(it => it.ShowCastMembers)
                .HasForeignKey(it => it.CastMemberId);
        }
    }
}
