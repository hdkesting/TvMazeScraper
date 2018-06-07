// <copyright file = "MockShowContext.cs" company="Hans Kesting">
// Copyright (c) Hans Kesting. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Core.Test.Mock
{
    using System;
    using System.Data.Entity;
    using System.Linq;
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
            FakeDatabaseRelation();
            return 0;
        }

        public Task<int> SaveChangesAsync()
        {
            FakeDatabaseRelation();
            return Task.FromResult(0);
        }

        /// <summary>
        /// Matches the shows and cast to fake the relations in the database.
        /// </summary>
        /// <exception cref="InvalidOperationException">Unknown showId for a cast member.</exception>
        private void FakeDatabaseRelation()
        {
            // check for unique keys
            if (this.Shows.GroupBy(s => s.Id).Where(g => g.Count() > 1).Any())
            {
                throw new InvalidOperationException("There are shows with duplicate IDs.");
            }

            if (this.CastMembers.GroupBy(m => new { m.ShowId, m.MemberId }).Where(g => g.Count() > 1).Any())
            {
                throw new InvalidOperationException("There are cast members with duplicate ID pairs.");
            }

            // make sure all castmembers belong to the correct show
            foreach (var member in this.CastMembers)
            {
                var show = this.Shows.SingleOrDefault(s => s.Id == member.ShowId);
                if (show != null)
                {
                    var castmember = show.CastMembers.SingleOrDefault(m => m.MemberId == member.MemberId);
                    if (castmember == null)
                    {
                        // member doesn't exist yet in show, so add
                        show.CastMembers.Add(member);
                    }
                    else if (castmember != member)
                    {
                        // different instance, so silently replace with "member"
                        show.CastMembers.Remove(castmember);
                        show.CastMembers.Add(member);
                    }
                    // else: same instance: great!
                }
                else
                {
                    throw new InvalidOperationException($"Found a member with memberID {member.MemberId} and showID {member.ShowId}, but that show doesn't exist.");
                }
            }

            // and that all shows' castmembers are listed in the separate castmembers list
            foreach (var show in this.Shows)
            {
                foreach (var castmember in show.CastMembers)
                {
                    var knownmember = this.CastMembers.SingleOrDefault(m => m.MemberId == castmember.MemberId && m.ShowId == castmember.ShowId);

                    if (knownmember == null)
                    {
                        this.CastMembers.Add(castmember);
                    }
                    // else it is the same instance (thanks to the previous code)
                }
            }
        }
    }
}
