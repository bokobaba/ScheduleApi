

using ScheduleApi.Data;
using ScheduleApi.Models;

namespace ScheduleApiTest.Helpers {
    public static class Utilities {
        public static List<Employee> seedingEmployees = new List<Employee>() {
            new Employee() {
                Name = "Parker",
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
            },
            new Employee() {
                Name = "Josh",
                EmployeeId = 1234567,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
            },
            new Employee() {
                Name = "InvalidUser",
                EmployeeId = 7654321,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
            },
            new Employee() {
                Name = "Test",
                EmployeeId = 1234566,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
            },
            new Employee() {
                Name = "Delete",
                EmployeeId = 1111111,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
            }

        };
        public static List<Request> seedingRequests = new List<Request>() {
            new Request() {
                EmployeeId = 6439174,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() {
                EmployeeId = 6439174,
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() {
                EmployeeId = 1234567,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() {
                EmployeeId = 1234567,
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() {
                EmployeeId = 7654321,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() {
                EmployeeId = 7654321,
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
        };
        public static List<Schedule> seedingSchedules = new List<Schedule>() {
            new Schedule() {
                EmployeeId = 6439174,
                Week = 3,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() {
                EmployeeId = 6439174,
                Week = 3,
                Year = 2022,
                Day = 2,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() {
                EmployeeId = 6439174,
                Week = 2,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() {
                EmployeeId = 1234567,
                Week = 2,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() {
                EmployeeId = 1234567,
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() {
                EmployeeId = 7654321,
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },

        };

        public static void InitializeDbForTests(ScheduleDbContext db) {
            db.Employees.AddRange(seedingEmployees);
            db.Requests.AddRange(seedingRequests);
            db.Schedules.AddRange(seedingSchedules);
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(ScheduleDbContext db) {
            db.Employees.RemoveRange(db.Employees);
            db.Requests.RemoveRange(db.Requests);
            db.Schedules.RemoveRange(db.Schedules);
            InitializeDbForTests(db);
        }
    }
}