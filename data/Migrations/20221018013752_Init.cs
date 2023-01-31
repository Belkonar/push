using System;
using Microsoft.EntityFrameworkCore.Migrations;
using shared.Models;

#nullable disable

namespace data.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "config",
                columns: table => new
                {
                    key = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_config", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "organization",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    contents = table.Column<Organization>(type: "jsonb", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organization", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "policy",
                columns: table => new
                {
                    key = table.Column<string>(type: "text", nullable: false),
                    policy = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_policy", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "thing",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    organization = table.Column<Guid>(type: "uuid", nullable: false),
                    contents = table.Column<Thing>(type: "jsonb", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_thing", x => x.id);
                    table.ForeignKey(
                        name: "FK_thing_organization_organization",
                        column: x => x.organization,
                        principalTable: "organization",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deployable",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    thing = table.Column<Guid>(type: "uuid", nullable: false),
                    contents = table.Column<Deployable>(type: "jsonb", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deployable", x => x.id);
                    table.ForeignKey(
                        name: "FK_deployable_thing_thing",
                        column: x => x.thing,
                        principalTable: "thing",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_deployable_thing",
                table: "deployable",
                column: "thing",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_thing_organization",
                table: "thing",
                column: "organization");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "config");

            migrationBuilder.DropTable(
                name: "deployable");

            migrationBuilder.DropTable(
                name: "policy");

            migrationBuilder.DropTable(
                name: "thing");

            migrationBuilder.DropTable(
                name: "organization");
        }
    }
}
