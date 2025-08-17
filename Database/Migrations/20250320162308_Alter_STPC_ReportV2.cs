using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class Alter_STPC_ReportV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string basePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            string[] sqlFiles = new string[]
            {
                Path.Combine(basePath, @"Database\SqlFiles\GetListDocumentsByUser.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\ReportApprovalByUser.sql"),
                Path.Combine(basePath, @"Database\SqlFiles\ReportDocumentApproval.sql"),
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
