using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Review_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrevId = table.Column<int>(type: "int", nullable: false),
                    NextId = table.Column<int>(type: "int", nullable: false),
                    DefaultUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSign = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewOrder", x => x.Id);
                },
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "ReviewOrderGroupDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewOrderId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DefaultUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewOrderGroupDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewOrderGroupDetail_ReviewOrder",
                        column: x => x.ReviewOrderId,
                        principalTable: "ReviewOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "ReviewOrderUserDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReviewOrderId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewOrderUserDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewOrderUserDetail_ReviewOrder",
                        column: x => x.ReviewOrderId,
                        principalTable: "ReviewOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                schema: "dbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
