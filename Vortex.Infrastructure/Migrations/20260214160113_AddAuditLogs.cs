using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vortex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangeType = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentEntityType = table.Column<string>(type: "text", nullable: true),
                    ParentEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OldValues = table.Column<string>(type: "jsonb", nullable: true),
                    NewValues = table.Column<string>(type: "jsonb", nullable: true),
                    AffectedColumns = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_audit_logs_CorrelationId",
                table: "tbl_audit_logs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_audit_logs_DateTime",
                table: "tbl_audit_logs",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_audit_logs_EntityId",
                table: "tbl_audit_logs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_audit_logs_ParentEntityId",
                table: "tbl_audit_logs",
                column: "ParentEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_audit_logs");
        }
    }
}
