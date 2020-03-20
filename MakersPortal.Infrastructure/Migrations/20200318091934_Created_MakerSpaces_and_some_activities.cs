using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MakersPortal.Infrastructure.Migrations
{
    public partial class Created_MakerSpaces_and_some_activities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MakerSpaces",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MakerSpaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Totems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<int>(nullable: false),
                    MakerSpaceId = table.Column<int>(nullable: false),
                    AuthToken = table.Column<Guid>(nullable: false),
                    LastSeen = table.Column<DateTime>(nullable: true),
                    Otp = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Totems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Totems_MakerSpaces_MakerSpaceId",
                        column: x => x.MakerSpaceId,
                        principalTable: "MakerSpaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaseActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    StartTotemId = table.Column<int>(nullable: true),
                    EndTotemId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseActivities_Totems_EndTotemId",
                        column: x => x.EndTotemId,
                        principalTable: "Totems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseActivities_Totems_StartTotemId",
                        column: x => x.StartTotemId,
                        principalTable: "Totems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BaseActivities_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaseActivities_EndTotemId",
                table: "BaseActivities",
                column: "EndTotemId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseActivities_StartTotemId",
                table: "BaseActivities",
                column: "StartTotemId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseActivities_UserId",
                table: "BaseActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Totems_MakerSpaceId",
                table: "Totems",
                column: "MakerSpaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaseActivities");

            migrationBuilder.DropTable(
                name: "Totems");

            migrationBuilder.DropTable(
                name: "MakerSpaces");
        }
    }
}
