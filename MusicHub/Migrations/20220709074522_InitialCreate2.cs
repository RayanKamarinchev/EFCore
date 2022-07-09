using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicHub.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SongPerformers_Performers_PerformerId",
                table: "SongPerformers");

            migrationBuilder.DropForeignKey(
                name: "FK_SongPerformers_Songs_SongId",
                table: "SongPerformers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SongPerformers",
                table: "SongPerformers");

            migrationBuilder.RenameTable(
                name: "SongPerformers",
                newName: "SongsPerformers");

            migrationBuilder.RenameIndex(
                name: "IX_SongPerformers_SongId",
                table: "SongsPerformers",
                newName: "IX_SongsPerformers_SongId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SongsPerformers",
                table: "SongsPerformers",
                columns: new[] { "PerformerId", "SongId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SongsPerformers_Performers_PerformerId",
                table: "SongsPerformers",
                column: "PerformerId",
                principalTable: "Performers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongsPerformers_Songs_SongId",
                table: "SongsPerformers",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SongsPerformers_Performers_PerformerId",
                table: "SongsPerformers");

            migrationBuilder.DropForeignKey(
                name: "FK_SongsPerformers_Songs_SongId",
                table: "SongsPerformers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SongsPerformers",
                table: "SongsPerformers");

            migrationBuilder.RenameTable(
                name: "SongsPerformers",
                newName: "SongPerformers");

            migrationBuilder.RenameIndex(
                name: "IX_SongsPerformers_SongId",
                table: "SongPerformers",
                newName: "IX_SongPerformers_SongId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SongPerformers",
                table: "SongPerformers",
                columns: new[] { "PerformerId", "SongId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SongPerformers_Performers_PerformerId",
                table: "SongPerformers",
                column: "PerformerId",
                principalTable: "Performers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SongPerformers_Songs_SongId",
                table: "SongPerformers",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
