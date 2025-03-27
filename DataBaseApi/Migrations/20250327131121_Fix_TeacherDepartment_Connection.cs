using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseApi.Migrations
{
    /// <inheritdoc />
    public partial class Fix_TeacherDepartment_Connection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Departments_DepartmentId1",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_DepartmentId1",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "DepartmentId1",
                table: "Teachers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId1",
                table: "Teachers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_DepartmentId1",
                table: "Teachers",
                column: "DepartmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Departments_DepartmentId1",
                table: "Teachers",
                column: "DepartmentId1",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
