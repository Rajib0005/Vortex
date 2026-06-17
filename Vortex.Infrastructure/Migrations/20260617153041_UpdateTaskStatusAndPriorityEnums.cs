using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vortex.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskStatusAndPriorityEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update Priority:
            // Old Priority values: Highest = 0, High = 1, Medium = 2, Low = 3, Lowest = 4
            // New Priority values: Low = 0, Medium = 1, High = 2, Urgent = 3
            migrationBuilder.Sql(@"
                UPDATE tbl_task_master 
                SET ""Priority"" = CASE 
                    WHEN ""Priority"" = 0 THEN 3
                    WHEN ""Priority"" = 1 THEN 2
                    WHEN ""Priority"" = 2 THEN 1
                    WHEN ""Priority"" = 3 THEN 0
                    WHEN ""Priority"" = 4 THEN 0
                    ELSE 1
                END;
            ");

            // Update Status:
            // Old Status values: Todo = 0, InProgress = 1, OnReview = 2, OnTest = 3, Done = 4
            // New Status values: Backlog = 0, Todo = 1, InProgress = 2, Done = 3, Canceled = 4
            migrationBuilder.Sql(@"
                UPDATE tbl_task_master 
                SET ""Status"" = CASE 
                    WHEN ""Status"" = 0 THEN 1
                    WHEN ""Status"" = 1 THEN 2
                    WHEN ""Status"" = 2 THEN 2
                    WHEN ""Status"" = 3 THEN 2
                    WHEN ""Status"" = 4 THEN 3
                    ELSE 1
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse Priority mapping:
            // New Priority values: Low = 0, Medium = 1, High = 2, Urgent = 3
            // Old Priority values: Highest = 0, High = 1, Medium = 2, Low = 3, Lowest = 4
            migrationBuilder.Sql(@"
                UPDATE tbl_task_master 
                SET ""Priority"" = CASE 
                    WHEN ""Priority"" = 3 THEN 0
                    WHEN ""Priority"" = 2 THEN 1
                    WHEN ""Priority"" = 1 THEN 2
                    WHEN ""Priority"" = 0 THEN 3
                    ELSE 2
                END;
            ");

            // Reverse Status mapping:
            // New Status values: Backlog = 0, Todo = 1, InProgress = 2, Done = 3, Canceled = 4
            // Old Status values: Todo = 0, InProgress = 1, OnReview = 2, OnTest = 3, Done = 4
            migrationBuilder.Sql(@"
                UPDATE tbl_task_master 
                SET ""Status"" = CASE 
                    WHEN ""Status"" = 0 THEN 0
                    WHEN ""Status"" = 1 THEN 0
                    WHEN ""Status"" = 2 THEN 1
                    WHEN ""Status"" = 3 THEN 4
                    WHEN ""Status"" = 4 THEN 0
                    ELSE 0
                END;
            ");
        }
    }
}
