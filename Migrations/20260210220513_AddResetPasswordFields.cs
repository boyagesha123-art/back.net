using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthyBites.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddResetPasswordFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "AppUsers",
                newName: "PasswordHash");

            migrationBuilder.AlterColumn<float>(
                name: "Value",
                table: "SugarReadings",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordToken",
                table: "AppUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetTokenExpiry",
                table: "AppUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPasswordToken",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "ResetTokenExpiry",
                table: "AppUsers");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "AppUsers",
                newName: "Password");

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "SugarReadings",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
