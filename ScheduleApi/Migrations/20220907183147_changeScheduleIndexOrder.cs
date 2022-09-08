using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApi.Migrations
{
    public partial class changeScheduleIndexOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_EmployeeId_Day_Week_Year",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_EmployeeId",
                table: "Schedules",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Year_Week_Day_EmployeeId",
                table: "Schedules",
                columns: new[] { "Year", "Week", "Day", "EmployeeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_EmployeeId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_Year_Week_Day_EmployeeId",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_EmployeeId_Day_Week_Year",
                table: "Schedules",
                columns: new[] { "EmployeeId", "Day", "Week", "Year" },
                unique: true);
        }
    }
}
