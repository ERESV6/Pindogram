using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace pindogramApp.Migrations
{
    public partial class Memes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Memes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    AuthorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memes_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemeRates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: true),
                    MemeId = table.Column<int>(nullable: true),
                    isUpvote = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemeRates_Memes_MemeId",
                        column: x => x.MemeId,
                        principalTable: "Memes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemeRates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemeRates_MemeId",
                table: "MemeRates",
                column: "MemeId");

            migrationBuilder.CreateIndex(
                name: "IX_MemeRates_UserId",
                table: "MemeRates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Memes_AuthorId",
                table: "Memes",
                column: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemeRates");

            migrationBuilder.DropTable(
                name: "Memes");
        }
    }
}
