using Microsoft.EntityFrameworkCore.Migrations;

namespace RtlTvMazeScraper.Infrastructure.Migrations
{
    public partial class ShowCastMember : Migration
    {
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
