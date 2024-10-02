using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    /// <inheritdoc />
    public partial class cancellationReasonAddedInsteadOfRefundReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundReason",
                table: "Refunds");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefundDate",
                table: "Refunds",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "CancellationInfos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_BookingId",
                table: "Refunds",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_UserId",
                table: "Refunds",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Bookings_BookingId",
                table: "Refunds",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Users_UserId",
                table: "Refunds",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Bookings_BookingId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Users_UserId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_BookingId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_UserId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "CancellationInfos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefundDate",
                table: "Refunds",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefundReason",
                table: "Refunds",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
