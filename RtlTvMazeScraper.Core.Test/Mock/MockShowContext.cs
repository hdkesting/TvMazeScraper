// <copyright file = "MockShowContext.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test.Mock
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using RtlTvMazeScraper.Core.Interfaces;
    using RtlTvMazeScraper.Core.Model;

    /// <summary>
    /// A fake ShowContext for use in unittests.
    /// </summary>
    /// <seealso cref="RtlTvMazeScraper.Core.Interfaces.IShowContext" />
    public class MockShowContext : IShowContext
    {
        /// <summary>
        /// Gets the shows.
        /// </summary>
        /// <value>
        /// The shows.
        /// </value>
        public DbSet<Show> Shows { get; } = new MockDbSet<Show>();

        /// <summary>
        /// Gets the cast members.
        /// </summary>
        /// <value>
        /// The cast members.
        /// </value>
        public DbSet<CastMember> CastMembers { get; } = new MockDbSet<CastMember>();

        public int SaveChanges()
        {
            return 0;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(0);
        }
    }
}
