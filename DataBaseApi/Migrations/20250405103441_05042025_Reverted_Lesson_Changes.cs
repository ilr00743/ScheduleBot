using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseApi.Migrations
{
    /// <inheritdoc />
    public partial class _05042025_Reverted_Lesson_Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuditoriumId1",
                table: "Lessons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayId1",
                table: "Lessons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisciplineId1",
                table: "Lessons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId1",
                table: "Lessons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId1",
                table: "Lessons",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_AuditoriumId1",
                table: "Lessons",
                column: "AuditoriumId1");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_DayId1",
                table: "Lessons",
                column: "DayId1");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_DisciplineId1",
                table: "Lessons",
                column: "DisciplineId1");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_GroupId1",
                table: "Lessons",
                column: "GroupId1");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_TeacherId1",
                table: "Lessons",
                column: "TeacherId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Auditoriums_AuditoriumId1",
                table: "Lessons",
                column: "AuditoriumId1",
                principalTable: "Auditoriums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Days_DayId1",
                table: "Lessons",
                column: "DayId1",
                principalTable: "Days",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Disciplines_DisciplineId1",
                table: "Lessons",
                column: "DisciplineId1",
                principalTable: "Disciplines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Groups_GroupId1",
                table: "Lessons",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Teachers_TeacherId1",
                table: "Lessons",
                column: "TeacherId1",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Auditoriums_AuditoriumId1",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Days_DayId1",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Disciplines_DisciplineId1",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Groups_GroupId1",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Teachers_TeacherId1",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_AuditoriumId1",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_DayId1",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_DisciplineId1",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_GroupId1",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_TeacherId1",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "AuditoriumId1",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "DayId1",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "DisciplineId1",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "Lessons");
        }
    }
}
