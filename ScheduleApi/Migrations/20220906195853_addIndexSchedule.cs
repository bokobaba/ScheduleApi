using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApi.Migrations
{
    public partial class addIndexSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Schedules_EmployeeId_Day_Week_Year",
                table: "Schedules",
                columns: new[] { "EmployeeId", "Day", "Week", "Year" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Employees_EmployeeId",
                table: "Schedules",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Employees_EmployeeId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_EmployeeId_Day_Week_Year",
                table: "Schedules");
        }
    }
}
