using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vortex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "tbl_audit_logs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tbl_comment_master",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_comment_master", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_comment_master_tbl_comment_master_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "tbl_comment_master",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_comment_master_tbl_project_master_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "tbl_project_master",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tbl_comment_master_tbl_task_master_TaskId",
                        column: x => x.TaskId,
                        principalTable: "tbl_task_master",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_comment_master_ParentCommentId",
                table: "tbl_comment_master",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_comment_master_ProjectId",
                table: "tbl_comment_master",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_comment_master_TaskId",
                table: "tbl_comment_master",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_comment_master");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "tbl_audit_logs");
        }
    }
}
