using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaturdayMP.Examples.TpsReportUploader.Migrations
{
    /// <inheritdoc />
    public partial class CreateReportProcessTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportProcessRuns",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RunDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportProcessRuns", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReportProcessRunItems",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ReportName = table.Column<string>(type: "TEXT", nullable: false),
                    ReportProcessRunID = table.Column<int>(type: "INTEGER", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportProcessRunItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReportProcessRunItems_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportProcessRunItems_ReportProcessRuns_ReportProcessRunID",
                        column: x => x.ReportProcessRunID,
                        principalTable: "ReportProcessRuns",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportProcessRunItems_ApplicationUserId",
                table: "ReportProcessRunItems",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportProcessRunItems_ReportProcessRunID",
                table: "ReportProcessRunItems",
                column: "ReportProcessRunID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportProcessRunItems");

            migrationBuilder.DropTable(
                name: "ReportProcessRuns");
        }
    }
}
