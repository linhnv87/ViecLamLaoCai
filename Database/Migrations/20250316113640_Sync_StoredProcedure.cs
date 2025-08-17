using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Sync_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            string[] sqlFiles = new string[]
            {
                Path.Combine(basePath, @"Database\SqlFiles\GetAllAppUsers.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\GetListDocumentsByUser.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\GetReviewReport.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\GetTotalOfReviews.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\GetUsersByDepartmentId.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\GetUsersByGroupId.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\GetUsersByRole.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\GetUsersByWorkFlow.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\ReportApprovalByUser.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\ReportDocumentApproval.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\RetrieveDocument.sql"),
            };

            foreach (var sqlFile in sqlFiles)
            {
                migrationBuilder.Sql(File.ReadAllText(sqlFile));
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
