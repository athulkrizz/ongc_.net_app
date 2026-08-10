using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ONGC.MilestoneAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMilestoneTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Milestones_Designs_DesignId",
                table: "Milestones");

            migrationBuilder.DropTable(
                name: "Designs");

            migrationBuilder.DropTable(
                name: "Wellbores");

            migrationBuilder.DropTable(
                name: "Wells");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Milestone_DesignId_MilestoneType",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_DesignId",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_Timestamp",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "DesignId",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "WorkCentre",
                table: "Milestones");

            migrationBuilder.RenameColumn(
                name: "MilestoneType",
                table: "Milestones",
                newName: "Days");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalLevel",
                table: "Milestones",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Asset",
                table: "Milestones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrentMilestone",
                table: "Milestones",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "PercentCompleted",
                table: "Milestones",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Milestones",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Milestones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Well",
                table: "Milestones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Wellbore",
                table: "Milestones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Asset",
                table: "Milestones",
                column: "Asset");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Status",
                table: "Milestones",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Well",
                table: "Milestones",
                column: "Well");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Wellbore",
                table: "Milestones",
                column: "Wellbore");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Milestones_Asset",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_Status",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_Well",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_Wellbore",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "ApprovalLevel",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "Asset",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "CurrentMilestone",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "PercentCompleted",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "User",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "Well",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "Wellbore",
                table: "Milestones");

            migrationBuilder.RenameColumn(
                name: "Days",
                table: "Milestones",
                newName: "MilestoneType");

            migrationBuilder.AddColumn<int>(
                name: "DesignId",
                table: "Milestones",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Milestones",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "Milestones",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "WorkCentre",
                table: "Milestones",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompanyId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    Location = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sites_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wells",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SiteId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    WellType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wells", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wells_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Wellbores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WellId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    DesignType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wellbores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wellbores_Wells_WellId",
                        column: x => x.WellId,
                        principalTable: "Wells",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Designs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WellboreId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Owner = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Designs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Designs_Wellbores_WellboreId",
                        column: x => x.WellboreId,
                        principalTable: "Wellbores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Milestone_DesignId_MilestoneType",
                table: "Milestones",
                columns: new[] { "DesignId", "MilestoneType" });

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_DesignId",
                table: "Milestones",
                column: "DesignId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Timestamp",
                table: "Milestones",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Design_Status_UpdatedDate",
                table: "Designs",
                columns: new[] { "Status", "UpdatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Designs_WellboreId",
                table: "Designs",
                column: "WellboreId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CompanyId",
                table: "Projects",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_ProjectId",
                table: "Sites",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Wellbores_WellId",
                table: "Wellbores",
                column: "WellId");

            migrationBuilder.CreateIndex(
                name: "IX_Wells_SiteId",
                table: "Wells",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Milestones_Designs_DesignId",
                table: "Milestones",
                column: "DesignId",
                principalTable: "Designs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
