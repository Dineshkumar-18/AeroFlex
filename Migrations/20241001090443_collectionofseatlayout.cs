using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class collectionofseatlayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeatLayouts_FlightId",
                table: "SeatLayouts");

            migrationBuilder.CreateIndex(
                name: "IX_SeatLayouts_FlightId",
                table: "SeatLayouts",
                column: "FlightId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeatLayouts_FlightId",
                table: "SeatLayouts");

            migrationBuilder.CreateIndex(
                name: "IX_SeatLayouts_FlightId",
                table: "SeatLayouts",
                column: "FlightId",
                unique: true);
        }
    }
}
