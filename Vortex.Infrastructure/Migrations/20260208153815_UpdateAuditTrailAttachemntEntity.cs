using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vortex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditTrailAttachemntEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "tbl_attachment_master");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "tbl_attachment_master");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "tbl_attachment_master",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "tbl_attachment_master",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "tbl_attachment_master");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "tbl_attachment_master");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "tbl_attachment_master",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "tbl_attachment_master",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
