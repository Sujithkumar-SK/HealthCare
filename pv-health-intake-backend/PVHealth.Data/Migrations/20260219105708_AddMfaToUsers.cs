using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PVHealth.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMfaToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackupCodes",
                table: "users",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MfaEnabled",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MfaSecret",
                table: "users",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackupCodes",
                table: "users");

            migrationBuilder.DropColumn(
                name: "MfaEnabled",
                table: "users");

            migrationBuilder.DropColumn(
                name: "MfaSecret",
                table: "users");
        }
    }
}
