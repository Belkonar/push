using System;
using Microsoft.EntityFrameworkCore.Migrations;
using shared.Models;
using shared.Models.Pipeline;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
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
                name: "pipeline_version",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    contents = table.Column<PipelineVersionContents>(type: "jsonb", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pipeline_version", x => x.id);
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
                name: "pipeline",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    organization = table.Column<Guid>(type: "uuid", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pipeline", x => x.id);
                    table.ForeignKey(
                        name: "FK_pipeline_organization_organization",
                        column: x => x.organization,
                        principalTable: "organization",
                        principalColumn: "id");
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
                name: "IX_pipeline_organization",
                table: "pipeline",
                column: "organization");

            migrationBuilder.CreateIndex(
                name: "IX_thing_organization",
                table: "thing",
                column: "organization");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "config");

            migrationBuilder.DropTable(
                name: "deployable");

            migrationBuilder.DropTable(
                name: "pipeline");

            migrationBuilder.DropTable(
                name: "pipeline_version");

            migrationBuilder.DropTable(
                name: "policy");

            migrationBuilder.DropTable(
                name: "thing");

            migrationBuilder.DropTable(
                name: "organization");
        }
    }
}
