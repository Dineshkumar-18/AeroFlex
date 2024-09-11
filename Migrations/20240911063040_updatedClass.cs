using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class updatedClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "FlightScheduleClasses");

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "FlightScheduleClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FlightScheduleClasses_ClassId",
                table: "FlightScheduleClasses",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightScheduleClasses_Classes_ClassId",
                table: "FlightScheduleClasses",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "ClassId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightScheduleClasses_Classes_ClassId",
                table: "FlightScheduleClasses");

            migrationBuilder.DropIndex(
                name: "IX_FlightScheduleClasses_ClassId",
                table: "FlightScheduleClasses");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "FlightScheduleClasses");

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "FlightScheduleClasses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
