using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DmhyAutoDownload.Migrations
{
    /// <inheritdoc />
    public partial class CreateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bangumis",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Regex = table.Column<string>(type: "TEXT", nullable: false),
                    RegexGroupIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    QueryKeyWord = table.Column<string>(type: "TEXT", nullable: false),
                    DownloadedEps = table.Column<string>(type: "TEXT", nullable: false),
                    Finished = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bangumis", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bangumis_Finished",
                table: "Bangumis",
                column: "Finished");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bangumis");
        }
    }
}
