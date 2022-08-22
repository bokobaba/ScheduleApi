using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApi.Migrations
{
    public partial class addedEmployeeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Schedules",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Requests",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Employees",
                newName: "ID");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Schedules",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Requests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Employees",
                newName: "Id");
        }
    }
}
