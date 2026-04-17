using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddAiFeedbackToExamAttempt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiFeedback",
                table: "ExamAttempts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AiFeedbackGeneratedAt",
                table: "ExamAttempts",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiFeedback",
                table: "ExamAttempts");

            migrationBuilder.DropColumn(
                name: "AiFeedbackGeneratedAt",
                table: "ExamAttempts");
        }
    }
}
