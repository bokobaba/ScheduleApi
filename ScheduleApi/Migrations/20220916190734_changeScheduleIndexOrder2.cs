using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApi.Migrations
{
    public partial class changeScheduleIndexOrder2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_Week_Year_UserId_EmployeeId_Day",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_UserId_Week_Year_EmployeeId_Day",
                table: "Schedules",
                columns: new[] { "UserId", "Week", "Year", "EmployeeId", "Day" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_UserId_Week_Year_EmployeeId_Day",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Week_Year_UserId_EmployeeId_Day",
                table: "Schedules",
                columns: new[] { "Week", "Year", "UserId", "EmployeeId", "Day" },
                unique: true);
        }
    }
}
