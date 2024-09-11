using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class updationCountryTax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CountryTax_Countries_CountryId",
                table: "CountryTax");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightsPricings_CountryTax_CountryTaxId",
                table: "FlightsPricings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CountryTax",
                table: "CountryTax");

            migrationBuilder.RenameTable(
                name: "CountryTax",
                newName: "CountryTaxes");

            migrationBuilder.RenameIndex(
                name: "IX_CountryTax_CountryId",
                table: "CountryTaxes",
                newName: "IX_CountryTaxes_CountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CountryTaxes",
                table: "CountryTaxes",
                column: "CountryTaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_CountryTaxes_Countries_CountryId",
                table: "CountryTaxes",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightsPricings_CountryTaxes_CountryTaxId",
                table: "FlightsPricings",
                column: "CountryTaxId",
                principalTable: "CountryTaxes",
                principalColumn: "CountryTaxId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CountryTaxes_Countries_CountryId",
                table: "CountryTaxes");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightsPricings_CountryTaxes_CountryTaxId",
                table: "FlightsPricings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CountryTaxes",
                table: "CountryTaxes");

            migrationBuilder.RenameTable(
                name: "CountryTaxes",
                newName: "CountryTax");

            migrationBuilder.RenameIndex(
                name: "IX_CountryTaxes_CountryId",
                table: "CountryTax",
                newName: "IX_CountryTax_CountryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CountryTax",
                table: "CountryTax",
                column: "CountryTaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_CountryTax_Countries_CountryId",
                table: "CountryTax",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "CountryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightsPricings_CountryTax_CountryTaxId",
                table: "FlightsPricings",
                column: "CountryTaxId",
                principalTable: "CountryTax",
                principalColumn: "CountryTaxId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
