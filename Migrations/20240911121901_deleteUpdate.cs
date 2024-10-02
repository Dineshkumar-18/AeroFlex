using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class deleteUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightSegments");

            migrationBuilder.DropTable(
                name: "Itineraries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Itineraries",
                columns: table => new
                {
                    ItineraryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndAirportId = table.Column<int>(type: "int", nullable: false),
                    StartAirportId = table.Column<int>(type: "int", nullable: false),
                    TotalStops = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itineraries", x => x.ItineraryId);
                    table.ForeignKey(
                        name: "FK_Itineraries_Airports_EndAirportId",
                        column: x => x.EndAirportId,
                        principalTable: "Airports",
                        principalColumn: "AirportId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Itineraries_Airports_StartAirportId",
                        column: x => x.StartAirportId,
                        principalTable: "Airports",
                        principalColumn: "AirportId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FlightSegments",
                columns: table => new
                {
                    FlightSegmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightScheduleId = table.Column<int>(type: "int", nullable: false),
                    ItineraryId = table.Column<int>(type: "int", nullable: true),
                    IsStop = table.Column<bool>(type: "bit", nullable: false),
                    StopOrder = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightSegments", x => x.FlightSegmentId);
                    table.ForeignKey(
                        name: "FK_FlightSegments_FlightsSchedules_FlightScheduleId",
                        column: x => x.FlightScheduleId,
                        principalTable: "FlightsSchedules",
                        principalColumn: "FlightScheduleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlightSegments_Itineraries_ItineraryId",
                        column: x => x.ItineraryId,
                        principalTable: "Itineraries",
                        principalColumn: "ItineraryId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightSegments_FlightScheduleId",
                table: "FlightSegments",
                column: "FlightScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSegments_ItineraryId",
                table: "FlightSegments",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Itineraries_EndAirportId",
                table: "Itineraries",
                column: "EndAirportId");

            migrationBuilder.CreateIndex(
                name: "IX_Itineraries_StartAirportId",
                table: "Itineraries",
                column: "StartAirportId");
        }
    }
}
