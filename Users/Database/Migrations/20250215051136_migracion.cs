using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apitienda.Migrations
{
    /// <inheritdoc />
    public partial class migracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    last_login = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_superuser = table.Column<bool>(type: "boolean", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    date_joined = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    username = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    profile_picture = table.Column<string>(type: "text", nullable: true),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    email_verified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    nationality = table.Column<string>(type: "text", nullable: true),
                    occupation = table.Column<string>(type: "text", nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    contact_phone_number = table.Column<string>(type: "text", nullable: true),
                    gender = table.Column<string>(type: "text", nullable: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    address_number = table.Column<string>(type: "text", nullable: true),
                    address_interior_number = table.Column<string>(type: "text", nullable: true),
                    address_complement = table.Column<string>(type: "text", nullable: true),
                    address_neighborhood = table.Column<string>(type: "text", nullable: true),
                    address_zip_code = table.Column<string>(type: "text", nullable: true),
                    address_city = table.Column<string>(type: "text", nullable: true),
                    address_state = table.Column<string>(type: "text", nullable: true),
                    role = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
