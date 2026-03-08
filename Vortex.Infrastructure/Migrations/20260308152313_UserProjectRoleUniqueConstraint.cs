using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vortex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserProjectRoleUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_user_project_master_ProjectId",
                table: "tbl_user_project_master");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_project_master_ProjectId_UserId",
                table: "tbl_user_project_master",
                columns: new[] { "ProjectId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tbl_user_project_master_ProjectId_UserId",
                table: "tbl_user_project_master");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_user_project_master_ProjectId",
                table: "tbl_user_project_master",
                column: "ProjectId");
        }
    }
}
