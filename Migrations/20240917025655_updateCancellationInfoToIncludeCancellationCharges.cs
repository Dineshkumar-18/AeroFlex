using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class updateCancellationInfoToIncludeCancellationCharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CancellationCharge",
                table: "CancellationInfos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PlatformAndServiceCharge",
                table: "CancellationInfos",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationCharge",
                table: "CancellationInfos");

            migrationBuilder.DropColumn(
                name: "PlatformAndServiceCharge",
                table: "CancellationInfos");
        }
    }
}
