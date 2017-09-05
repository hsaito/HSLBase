using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HSLProcessor.Migrations
{
    // ReSharper disable once UnusedMember.Global
    public partial class Songs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Artists",
                table => new
                {
                    ArtistId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Artists", x => x.ArtistId); });

            migrationBuilder.CreateTable(
                "Series",
                table => new
                {
                    SeriesId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Series", x => x.SeriesId); });

            migrationBuilder.CreateTable(
                "Sources",
                table => new
                {
                    SourceId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SeriesId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.SourceId);
                    table.ForeignKey(
                        "FK_Sources_Series_SeriesId",
                        x => x.SeriesId,
                        "Series",
                        "SeriesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Songs",
                table => new
                {
                    TitleId = table.Column<Guid>(nullable: false),
                    ArtistId = table.Column<Guid>(nullable: false),
                    SourceId = table.Column<Guid>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.TitleId);
                    table.ForeignKey(
                        "FK_Songs_Artists_ArtistId",
                        x => x.ArtistId,
                        "Artists",
                        "ArtistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Songs_Sources_SourceId",
                        x => x.SourceId,
                        "Sources",
                        "SourceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Songs_ArtistId",
                "Songs",
                "ArtistId");

            migrationBuilder.CreateIndex(
                "IX_Songs_SourceId",
                "Songs",
                "SourceId");

            migrationBuilder.CreateIndex(
                "IX_Sources_SeriesId",
                "Sources",
                "SeriesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Songs");

            migrationBuilder.DropTable(
                "Artists");

            migrationBuilder.DropTable(
                "Sources");

            migrationBuilder.DropTable(
                "Series");
        }
    }
}