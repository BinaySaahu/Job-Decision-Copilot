using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobDecisionEngine.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedResumeUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastParseError",
                table: "OnboardingProfiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParseStatus",
                table: "OnboardingProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ParsedAt",
                table: "OnboardingProfiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParsedResumeJson",
                table: "OnboardingProfiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastParseError",
                table: "OnboardingProfiles");

            migrationBuilder.DropColumn(
                name: "ParseStatus",
                table: "OnboardingProfiles");

            migrationBuilder.DropColumn(
                name: "ParsedAt",
                table: "OnboardingProfiles");

            migrationBuilder.DropColumn(
                name: "ParsedResumeJson",
                table: "OnboardingProfiles");
        }
    }
}
