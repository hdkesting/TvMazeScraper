// <copyright file="ShowContextFactory.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace TvMazeScraper.Infrastructure.Sql.Model
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

#pragma warning disable CA1812 // Avoid uninstantiated internal classes. Disable because it is used by Migrations.
    /// <summary>
    /// Creates a <see cref="ShowContext"/> for use by "Migrations".
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation.
    /// </remarks>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory{TContext}" />
    /// <seealso cref="ShowContext"/>
    internal class ShowContextFactory : IDesignTimeDbContextFactory<ShowContext>
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>
        /// An instance of <see cref="ShowContext"/>.
        /// </returns>
        public ShowContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShowContext>();
            optionsBuilder.UseSqlServer(@"Server=.\\sqlexpress;Database=tvmaze;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;Application Name=TvMazeScraper");

            return new ShowContext(optionsBuilder.Options);
        }
    }
}
