using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class updationlatest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightsPricings_FlightTaxes_FlightTaxId",
                table: "FlightsPricings");

            migrationBuilder.RenameColumn(
                name: "FlightTaxId",
                table: "FlightsPricings",
                newName: "CountryTaxId");

            migrationBuilder.RenameIndex(
                name: "IX_FlightsPricings_FlightTaxId",
                table: "FlightsPricings",
                newName: "IX_FlightsPricings_CountryTaxId");

            migrationBuilder.CreateTable(
                name: "CountryTax",
                columns: table => new
                {
                    CountryTaxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    TravelType = table.Column<int>(type: "int", nullable: false),
                    CountryTaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryTax", x => x.CountryTaxId);
                    table.ForeignKey(
                        name: "FK_CountryTax_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CountryTax_CountryId",
                table: "CountryTax",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightsPricings_CountryTax_CountryTaxId",
                table: "FlightsPricings",
                column: "CountryTaxId",
                principalTable: "CountryTax",
                principalColumn: "CountryTaxId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightsPricings_CountryTax_CountryTaxId",
                table: "FlightsPricings");

            migrationBuilder.DropTable(
                name: "CountryTax");

            migrationBuilder.RenameColumn(
                name: "CountryTaxId",
                table: "FlightsPricings",
                newName: "FlightTaxId");

            migrationBuilder.RenameIndex(
                name: "IX_FlightsPricings_CountryTaxId",
                table: "FlightsPricings",
                newName: "IX_FlightsPricings_FlightTaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightsPricings_FlightTaxes_FlightTaxId",
                table: "FlightsPricings",
                column: "FlightTaxId",
                principalTable: "FlightTaxes",
                principalColumn: "FlightTaxId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
