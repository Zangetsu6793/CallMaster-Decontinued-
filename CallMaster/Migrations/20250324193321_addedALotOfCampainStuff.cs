using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CallMaster.Migrations
{
    /// <inheritdoc />
    public partial class addedALotOfCampainStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "AutoDialEnabled",
                table: "Campaigns",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Configuration",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DaysToLookBack",
                table: "Campaigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastGenerated",
                table: "Campaigns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxAttemptsPerNumber",
                table: "Campaigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinCallsPerHour",
                table: "Campaigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrioritizeOlderCallbacks",
                table: "Campaigns",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RequiredCallbackRate",
                table: "Campaigns",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Campaigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CampaignPhoneNumbers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AutoDialEnabled",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Configuration",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "DaysToLookBack",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "LastGenerated",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "MaxAttemptsPerNumber",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "MinCallsPerHour",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "PrioritizeOlderCallbacks",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "RequiredCallbackRate",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CampaignPhoneNumbers");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
