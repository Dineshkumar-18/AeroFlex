using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class updaterelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CancellationInfos_CancellationFeeId",
                table: "CancellationInfos");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationInfos_CancellationFeeId",
                table: "CancellationInfos",
                column: "CancellationFeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CancellationInfos_CancellationFeeId",
                table: "CancellationInfos");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationInfos_CancellationFeeId",
                table: "CancellationInfos",
                column: "CancellationFeeId",
                unique: true);
        }
    }
}
