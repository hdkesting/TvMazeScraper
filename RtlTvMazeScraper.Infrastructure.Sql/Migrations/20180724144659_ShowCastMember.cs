// <copyright file="20180724144659_ShowCastMember.cs" company="Hans Keﬆing">
// Copyright (c) Hans Keﬆing. All rights reserved.
// </copyright>

namespace RtlTvMazeScraper.Infrastructure.Sql.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Migration to add many-to-many relationship "Show - CastMember".
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.Migrations.Migration" />
    public partial class ShowCastMember : Migration
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
        /// This method must be overridden in each class the inherits from <see cref="Microsoft.EntityFrameworkCore.Migrations.Migration" />.
        /// </para>
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder" /> that will build the operations.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // easy way out, just delete everything from this table. Start full import to re-load including the m2m relation table.
            migrationBuilder.Sql("delete from [dbo].[CastMembers]");

            migrationBuilder.DropForeignKey(
                name: "FK_CastMembers_Shows_ShowId",
                table: "CastMembers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_CastMembers_MemberId_ShowId",
                table: "CastMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CastMembers",
                table: "CastMembers");

            migrationBuilder.DropColumn(
                name: "ShowId",
                table: "CastMembers");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "CastMembers",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CastMembers",
                table: "CastMembers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ShowCastMembers",
                columns: table => new
                {
                    ShowId = table.Column<int>(nullable: false),
                    CastMemberId = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowCastMembers", x => new { x.ShowId, x.CastMemberId });
                    table.ForeignKey(
                        name: "FK_ShowCastMembers_CastMembers_CastMemberId",
                        column: x => x.CastMemberId,
                        principalTable: "CastMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShowCastMembers_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShowCastMembers_CastMemberId",
                table: "ShowCastMembers",
                column: "CastMemberId");
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
        /// This method must be overridden in each class the inherits from <see cref="Microsoft.EntityFrameworkCore.Migrations.Migration" /> if
        /// both 'up' and 'down' migrations are to be supported. If it is not overridden, then calling it
        /// will throw and it will not be possible to migrate in the 'down' direction.
        /// </para>
        /// </summary>
        /// <param name="migrationBuilder">The <see cref="Microsoft.EntityFrameworkCore.Migrations.MigrationBuilder" /> that will build the operations.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShowCastMembers");

            // easy way out, just delete everything from this table. Start full import to re-load including the m2m relation table.
            migrationBuilder.Sql("delete from [dbo].[CastMembers]");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CastMembers",
                table: "CastMembers");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CastMembers",
                newName: "MemberId");

            migrationBuilder.AddColumn<int>(
                name: "ShowId",
                table: "CastMembers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CastMembers_MemberId_ShowId",
                table: "CastMembers",
                columns: new[] { "MemberId", "ShowId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CastMembers",
                table: "CastMembers",
                columns: new[] { "ShowId", "MemberId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CastMembers_Shows_ShowId",
                table: "CastMembers",
                column: "ShowId",
                principalTable: "Shows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
