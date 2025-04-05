using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseApi.Migrations
{
    /// <inheritdoc />
    public partial class _05042025_Override_WeekDay_ToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_WeekDay_DayId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeekDay",
                table: "WeekDay");

            migrationBuilder.RenameTable(
                name: "WeekDay",
                newName: "Days");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Days",
                table: "Days",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Days_DayId",
                table: "Lessons",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Days_DayId",
                table: "Lessons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Days",
                table: "Days");

            migrationBuilder.RenameTable(
                name: "Days",
                newName: "WeekDay");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeekDay",
                table: "WeekDay",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_WeekDay_DayId",
                table: "Lessons",
                column: "DayId",
                principalTable: "WeekDay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
