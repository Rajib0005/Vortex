using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vortex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskEntityForTimeTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "tbl_task_master",
                newName: "DueDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "tbl_task_master",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Labels",
                table: "tbl_task_master",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "OriginalEstimateMinutes",
                table: "tbl_task_master",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingEstimateMinutes",
                table: "tbl_task_master",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReporterId",
                table: "tbl_task_master",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "tbl_task_master",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeSpentMinutes",
                table: "tbl_task_master",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_task_master_ReporterId",
                table: "tbl_task_master",
                column: "ReporterId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_task_master_tbl_user_master_ReporterId",
                table: "tbl_task_master",
                column: "ReporterId",
                principalTable: "tbl_user_master",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_task_master_tbl_user_master_ReporterId",
                table: "tbl_task_master");

            migrationBuilder.DropIndex(
                name: "IX_tbl_task_master_ReporterId",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "Labels",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "OriginalEstimateMinutes",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "RemainingEstimateMinutes",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "ReporterId",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "TimeSpentMinutes",
                table: "tbl_task_master");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "tbl_task_master",
                newName: "EndDate");
        }
    }
}
