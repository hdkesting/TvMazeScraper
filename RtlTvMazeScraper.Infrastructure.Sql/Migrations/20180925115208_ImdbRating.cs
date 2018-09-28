﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace TvMazeScraper.Infrastructure.Sql.Migrations
{
    /// <summary>
    /// Migration to add columns for IMDb rating.
    /// </summary>
    /// <seealso cref="Migration" />
    public partial class ImdbRating : Migration
    {
        /// <summary>
        /// <para>
        /// Builds the operations that will migrate the database 'up'.
        /// </para>
        /// <para>
        /// That is, builds the operations that will take the database from the state left in by the
        /// previous migration so that it is up-to-date with regard to this migration.
        /// </para>
        /// <para>
        /// This method must be overridden in each class the inherits from <see cref="Migration" />.
        /// </para>
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="MigrationBuilder" /> that will build the operations.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImdbId",
                table: "Shows",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ImdbRating",
                table: "Shows",
                nullable: true);
        }

        /// <summary>
        /// <para>
        /// Builds the operations that will migrate the database 'down'.
        /// </para>
        /// <para>
        /// That is, builds the operations that will take the database from the state left in by
        /// this migration so that it returns to the state that it was in before this migration was applied.
        /// </para>
        /// <para>
        /// This method must be overridden in each class the inherits from <see cref="Migration" /> if
        /// both 'up' and 'down' migrations are to be supported. If it is not overridden, then calling it
        /// will throw and it will not be possible to migrate in the 'down' direction.
        /// </para>
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="MigrationBuilder" /> that will build the operations.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImdbId",
                table: "Shows");

            migrationBuilder.DropColumn(
                name: "ImdbRating",
                table: "Shows");
        }
    }
}
