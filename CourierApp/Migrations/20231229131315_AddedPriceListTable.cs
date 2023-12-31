using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourierAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedPriceListTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "Shipments",
                type: "real",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PriceList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VerySmallSize = table.Column<float>(type: "real", nullable: false),
                    SmallSize = table.Column<float>(type: "real", nullable: false),
                    MediumSize = table.Column<float>(type: "real", nullable: false),
                    LargeSize = table.Column<float>(type: "real", nullable: false),
                    LightWeight = table.Column<float>(type: "real", nullable: false),
                    MediumWeight = table.Column<float>(type: "real", nullable: false),
                    HeavyWeight = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceList", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceList");
            
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Shipments");
        }
    }
}
