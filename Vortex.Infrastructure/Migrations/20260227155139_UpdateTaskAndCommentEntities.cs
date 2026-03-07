using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vortex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskAndCommentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "tbl_task_master",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "tbl_task_master",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoryPoints",
                table: "tbl_task_master",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskKey",
                table: "tbl_task_master",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LastTaskSequence",
                table: "tbl_project_master",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAt",
                table: "tbl_comment_master",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "tbl_comment_master",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TaskEntityId",
                table: "tbl_comment_master",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "tbl_project_master",
                keyColumn: "Id",
                keyValue: new Guid("7c7e06ed-80f7-4505-87e2-5191d13db645"),
                column: "LastTaskSequence",
                value: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tbl_task_master_AssigneeId",
                table: "tbl_task_master",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_comment_master_TaskEntityId",
                table: "tbl_comment_master",
                column: "TaskEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_comment_master_tbl_task_master_TaskEntityId",
                table: "tbl_comment_master",
                column: "TaskEntityId",
                principalTable: "tbl_task_master",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tbl_task_master_tbl_user_master_AssigneeId",
                table: "tbl_task_master",
                column: "AssigneeId",
                principalTable: "tbl_user_master",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tbl_comment_master_tbl_task_master_TaskEntityId",
                table: "tbl_comment_master");

            migrationBuilder.DropForeignKey(
                name: "FK_tbl_task_master_tbl_user_master_AssigneeId",
                table: "tbl_task_master");

            migrationBuilder.DropIndex(
                name: "IX_tbl_task_master_AssigneeId",
                table: "tbl_task_master");

            migrationBuilder.DropIndex(
                name: "IX_tbl_comment_master_TaskEntityId",
                table: "tbl_comment_master");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "StoryPoints",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "TaskKey",
                table: "tbl_task_master");

            migrationBuilder.DropColumn(
                name: "LastTaskSequence",
                table: "tbl_project_master");

            migrationBuilder.DropColumn(
                name: "EditedAt",
                table: "tbl_comment_master");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "tbl_comment_master");

            migrationBuilder.DropColumn(
                name: "TaskEntityId",
                table: "tbl_comment_master");
        }
    }
}
