using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class someupdatationoncancellationinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CancellationInfos_SeatId",
                table: "CancellationInfos");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationInfos_SeatId",
                table: "CancellationInfos",
                column: "SeatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CancellationInfos_SeatId",
                table: "CancellationInfos");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationInfos_SeatId",
                table: "CancellationInfos",
                column: "SeatId",
                unique: true);
        }
    }
}
