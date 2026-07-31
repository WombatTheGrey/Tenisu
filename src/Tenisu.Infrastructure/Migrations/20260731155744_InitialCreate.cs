using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenisu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sex = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(3)", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data_Rank = table.Column<int>(type: "int", nullable: false),
                    Data_Points = table.Column<int>(type: "int", nullable: false),
                    Data_Weight = table.Column<int>(type: "int", nullable: false),
                    Data_Height = table.Column<int>(type: "int", nullable: false),
                    Data_Age = table.Column<int>(type: "int", nullable: false),
                    Data_Last = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_CountryCode",
                table: "Players",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Players_Data_Rank",
                table: "Players",
                column: "Data_Rank",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_FirstName_LastName_Sex",
                table: "Players",
                columns: new[] { "FirstName", "LastName", "Sex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
