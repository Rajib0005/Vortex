using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vortex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProjectPriorityEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tbl_project_master",
                keyColumn: "Id",
                keyValue: new Guid("7c7e06ed-80f7-4505-87e2-5191d13db645"),
                column: "Priority",
                value: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "tbl_project_master",
                keyColumn: "Id",
                keyValue: new Guid("7c7e06ed-80f7-4505-87e2-5191d13db645"),
                column: "Priority",
                value: 2);
        }
    }
}
