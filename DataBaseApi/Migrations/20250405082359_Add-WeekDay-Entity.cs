using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataBaseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWeekDayEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "Lessons");

            migrationBuilder.AddColumn<int>(
                name: "DayId",
                table: "Lessons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WeekDay",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CodeAlias = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekDay", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WeekDay",
                columns: new[] { "Id", "CodeAlias", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Понеділок" },
                    { 2, 2, "Вівторок" },
                    { 3, 3, "Середа" },
                    { 4, 4, "Четвер" },
                    { 5, 5, "П'ятниця" },
                    { 6, 6, "Субота" },
                    { 7, 0, "Неділя" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_DayId",
                table: "Lessons",
                column: "DayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_WeekDay_DayId",
                table: "Lessons",
                column: "DayId",
                principalTable: "WeekDay",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_WeekDay_DayId",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "WeekDay");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_DayId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "DayId",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "Day",
                table: "Lessons",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
