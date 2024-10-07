using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class flightschedulelayout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalSeats",
                table: "FlightsSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FlightScheduleLayouts",
                columns: table => new
                {
                    SeatLayoutId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightScheduleId = table.Column<int>(type: "int", nullable: false),
                    TotalColumns = table.Column<int>(type: "int", nullable: false),
                    LayoutPattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatTypePattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RowCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightScheduleLayouts", x => x.SeatLayoutId);
                    table.ForeignKey(
                        name: "FK_FlightScheduleLayouts_FlightsSchedules_FlightScheduleId",
                        column: x => x.FlightScheduleId,
                        principalTable: "FlightsSchedules",
                        principalColumn: "FlightScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightScheduleUnavailableSeats",
                columns: table => new
                {
                    UnavailableSeatsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightScheduleUnavailableSeats", x => x.UnavailableSeatsId);
                    table.ForeignKey(
                        name: "FK_FlightScheduleUnavailableSeats_FlightsSchedules_FlightScheduleId",
                        column: x => x.FlightScheduleId,
                        principalTable: "FlightsSchedules",
                        principalColumn: "FlightScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightScheduleLayouts_FlightScheduleId",
                table: "FlightScheduleLayouts",
                column: "FlightScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightScheduleUnavailableSeats_FlightScheduleId",
                table: "FlightScheduleUnavailableSeats",
                column: "FlightScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightScheduleLayouts");

            migrationBuilder.DropTable(
                name: "FlightScheduleUnavailableSeats");

            migrationBuilder.DropColumn(
                name: "TotalSeats",
                table: "FlightsSchedules");
        }
    }
}
