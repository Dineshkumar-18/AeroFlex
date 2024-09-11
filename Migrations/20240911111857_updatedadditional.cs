using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class updatedadditional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightSegments_Itineraries_ItineraryId",
                table: "FlightSegments");

            migrationBuilder.AlterColumn<int>(
                name: "StopOrder",
                table: "FlightSegments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ItineraryId",
                table: "FlightSegments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightSegments_Itineraries_ItineraryId",
                table: "FlightSegments",
                column: "ItineraryId",
                principalTable: "Itineraries",
                principalColumn: "ItineraryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightSegments_Itineraries_ItineraryId",
                table: "FlightSegments");

            migrationBuilder.AlterColumn<int>(
                name: "StopOrder",
                table: "FlightSegments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ItineraryId",
                table: "FlightSegments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightSegments_Itineraries_ItineraryId",
                table: "FlightSegments",
                column: "ItineraryId",
                principalTable: "Itineraries",
                principalColumn: "ItineraryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
