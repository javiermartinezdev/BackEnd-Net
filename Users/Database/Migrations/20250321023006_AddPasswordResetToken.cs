using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apitienda.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password_reset_token",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "password_reset_token_expiration",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_reset_token",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "password_reset_token_expiration",
                table: "Users");
        }
    }
}
