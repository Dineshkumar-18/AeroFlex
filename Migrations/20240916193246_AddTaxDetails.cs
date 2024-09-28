using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Payments");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Seats",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "AdjustedSeatPrice",
                table: "FlightsPricings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "FlightsPricings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Seats");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "AdjustedSeatPrice",
                table: "FlightsPricings");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "FlightsPricings");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
