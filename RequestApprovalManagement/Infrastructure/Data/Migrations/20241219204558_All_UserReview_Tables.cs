using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class All_UserReview_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
             name: "ReviewProcesses",
             columns: table => new
             {
                 Id = table.Column<int>(type: "int", nullable: false)
                     .Annotation("SqlServer:Identity", "1, 1"),
                 CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                 DocumentId = table.Column<int>(type: "int", nullable: false),
                 ReviewDate = table.Column<DateTime>(type: "datetime", nullable: false),
                 DocumentStatus = table.Column<int>(type: "int", nullable: false),
                 Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                 Deadline = table.Column<DateTime>(type: "datetime", nullable: true)
             },
             constraints: table =>
             {
                 table.PrimaryKey("PK_ReviewProcesses", x => x.Id);
             },
             schema: "dbo");
            migrationBuilder.CreateTable(
              name: "ReviewProcessDetail",
              columns: table => new
              {
                  Id = table.Column<int>(type: "int", nullable: false)
                      .Annotation("SqlServer:Identity", "1, 1"),
                  CurrentProcessId = table.Column<int>(type: "int", nullable: false),
                  ReviewProcessId = table.Column<int>(type: "int", nullable: false),
                  ProcessStatus = table.Column<int>(type: "int", nullable: false),
                  CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                  UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                  ResultLinkDocumentId = table.Column<int>(type: "int", nullable: true),
                  Deadline = table.Column<DateTime>(type: "datetime", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_ReviewProcessDetail", x => x.Id);
                  table.ForeignKey(
                      name: "FK_ReviewProcessDetail_ReviewProcesses",
                      column: x => x.ReviewProcessId,
                      principalTable: "ReviewProcesses",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              },
              schema: "dbo");

            migrationBuilder.CreateTable(
                name: "ReviewUserDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewProcessDetailId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProcessStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    SignAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    SignedLinkDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultLinkDocumentId = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewUserDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewUserDetail_ReviewProcessDetail",
                        column: x => x.ReviewProcessDetailId,
                        principalTable: "ReviewProcessDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                schema: "dbo");

          

           

            migrationBuilder.CreateIndex(
                name: "IX_ReviewProcesses",
                table: "ReviewProcesses",
                column: "Id");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
