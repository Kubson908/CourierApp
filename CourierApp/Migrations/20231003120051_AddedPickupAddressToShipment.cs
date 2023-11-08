using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourierAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedPickupAddressToShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Shipments",
                newName: "RecipientPostalCode");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "Shipments",
                newName: "RecipientCity");

            migrationBuilder.RenameColumn(
                name: "ApartmentNumber",
                table: "Shipments",
                newName: "RecipientApartmentNumber");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Shipments",
                newName: "RecipientAddress");

            migrationBuilder.AddColumn<string>(
                name: "PickupAddress",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PickupApartmentNumber",
                table: "Shipments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupCity",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PickupPostalCode",
                table: "Shipments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupAddress",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "PickupApartmentNumber",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "PickupCity",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "PickupPostalCode",
                table: "Shipments");

            migrationBuilder.RenameColumn(
                name: "RecipientPostalCode",
                table: "Shipments",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "RecipientCity",
                table: "Shipments",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "RecipientApartmentNumber",
                table: "Shipments",
                newName: "ApartmentNumber");

            migrationBuilder.RenameColumn(
                name: "RecipientAddress",
                table: "Shipments",
                newName: "Address");
        }
    }
}
