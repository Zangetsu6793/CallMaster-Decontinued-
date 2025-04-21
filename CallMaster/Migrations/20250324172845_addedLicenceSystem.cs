using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CallMaster.Migrations
{
    /// <inheritdoc />
    public partial class addedLicenceSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LicenceKeyId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LicenceKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuableAcounts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenceKeys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_LicenceKeyId",
                table: "Users",
                column: "LicenceKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_LicenceKeys_LicenceKeyId",
                table: "Users",
                column: "LicenceKeyId",
                principalTable: "LicenceKeys",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_LicenceKeys_LicenceKeyId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "LicenceKeys");

            migrationBuilder.DropIndex(
                name: "IX_Users_LicenceKeyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LicenceKeyId",
                table: "Users");
        }
    }
}
