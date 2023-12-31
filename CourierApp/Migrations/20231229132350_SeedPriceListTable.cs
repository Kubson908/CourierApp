using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourierAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedPriceListTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PriceList",
                columns: new[] { "Id", "VerySmallSize", "SmallSize", "MediumSize", "LargeSize", "LightWeight", "MediumWeight", "HeavyWeight" },
                values: new object[,]
                {
                    { 1, 5, 10, 15, 20, 3.99f, 6.99f, 10.99f },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PriceList",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
