using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vortex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectEntityProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "tbl_project_master",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDeadline",
                table: "tbl_project_master",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "tbl_project_master",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "tbl_project_master",
                keyColumn: "Id",
                keyValue: new Guid("7c7e06ed-80f7-4505-87e2-5191d13db645"),
                columns: new[] { "Domain", "EstimatedDeadline", "Priority" },
                values: new object[] { "Development", null, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "tbl_project_master");

            migrationBuilder.DropColumn(
                name: "EstimatedDeadline",
                table: "tbl_project_master");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "tbl_project_master");
        }
    }
}
