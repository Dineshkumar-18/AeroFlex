using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class newthingsaredone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_CancellationInfos_CancellationId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_CancellationId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "CancellationId",
                table: "Refunds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CancellationId",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_CancellationId",
                table: "Refunds",
                column: "CancellationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_CancellationInfos_CancellationId",
                table: "Refunds",
                column: "CancellationId",
                principalTable: "CancellationInfos",
                principalColumn: "CancellationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
