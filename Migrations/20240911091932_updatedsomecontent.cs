using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class updatedsomecontent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlightTaxId",
                table: "FlightScheduleClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "FlightScheduleClasses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_FlightScheduleClasses_FlightTaxId",
                table: "FlightScheduleClasses",
                column: "FlightTaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightScheduleClasses_FlightTaxes_FlightTaxId",
                table: "FlightScheduleClasses",
                column: "FlightTaxId",
                principalTable: "FlightTaxes",
                principalColumn: "FlightTaxId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightScheduleClasses_FlightTaxes_FlightTaxId",
                table: "FlightScheduleClasses");

            migrationBuilder.DropIndex(
                name: "IX_FlightScheduleClasses_FlightTaxId",
                table: "FlightScheduleClasses");

            migrationBuilder.DropColumn(
                name: "FlightTaxId",
                table: "FlightScheduleClasses");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "FlightScheduleClasses");
        }
    }
}
