using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApi.Migrations
{
    public partial class changeScheduleIndexOrderAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_Year_Week_Day_EmployeeId",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Week_Year_EmployeeId_Day",
                table: "Schedules",
                columns: new[] { "Week", "Year", "EmployeeId", "Day" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_Week_Year_EmployeeId_Day",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Year_Week_Day_EmployeeId",
                table: "Schedules",
                columns: new[] { "Year", "Week", "Day", "EmployeeId" },
                unique: true);
        }
    }
}
