using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class updatation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightTaxes_Currencies_CurrencyId",
                table: "FlightTaxes");

            migrationBuilder.DropIndex(
                name: "IX_FlightTaxes_CurrencyId",
                table: "FlightTaxes");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "FlightTaxes");

            migrationBuilder.DropColumn(
                name: "TaxType",
                table: "FlightTaxes");

            migrationBuilder.AddColumn<int>(
                name: "TotalSeatColumn",
                table: "Flights",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalSeatColumn",
                table: "Flights");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "FlightTaxes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TaxType",
                table: "FlightTaxes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FlightTaxes_CurrencyId",
                table: "FlightTaxes",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightTaxes_Currencies_CurrencyId",
                table: "FlightTaxes",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "CurrencyId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
