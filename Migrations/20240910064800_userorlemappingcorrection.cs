using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroFlex.Migrations
{
    public partial class userorlemappingcorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing primary key constraint
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleMappings",
                table: "RoleMappings");

            // Drop the existing index on UserId
            migrationBuilder.DropIndex(
                name: "IX_RoleMappings_UserId",
                table: "RoleMappings");

            // Make changes to columns (if necessary) but do not attempt to modify IDENTITY
            migrationBuilder.AlterColumn<int>(
                name: "UserRoleMappingId",
                table: "RoleMappings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Apply other column changes as needed
            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "RefreshTokenInfos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationTime",
                table: "RefreshTokenInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Set the primary key to a composite key (UserId and RoleId)
            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleMappings",
                table: "RoleMappings",
                columns: new[] { "UserId", "RoleId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the composite primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleMappings",
                table: "RoleMappings");

            // Remove the newly added column
            migrationBuilder.DropColumn(
                name: "ExpirationTime",
                table: "RefreshTokenInfos");

            // Revert column changes to their previous state
            migrationBuilder.AlterColumn<int>(
                name: "UserRoleMappingId",
                table: "RoleMappings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "RefreshTokenInfos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Recreate the primary key on UserRoleMappingId
            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleMappings",
                table: "RoleMappings",
                column: "UserRoleMappingId");

            // Recreate the index on UserId
            migrationBuilder.CreateIndex(
                name: "IX_RoleMappings_UserId",
                table: "RoleMappings",
                column: "UserId");
        }
    }
}
