

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using ScheduleApi.Data;
using ScheduleApi.Models;
using ScheduleApiTest.Models;

namespace ScheduleApiTest.Helpers {
    public static class Utilities {
        public static IConfigurationRoot config;

        public static List<Employee> seedingEmployees = new List<Employee>() {
            new Employee() {
                Name = "Parker",
                EmployeeId = 6439174,
            },
            new Employee() {
                Name = "Josh",
                EmployeeId = 1234567,
            },
            new Employee() {
                Name = "InvalidUser",
                EmployeeId = 7654321,
                UserId = "invalid user",
            },
            new Employee() {
                Name = "Test Update",
                EmployeeId = 1234566,
            },
            new Employee() {
                Name = "Delete",
                EmployeeId = 1111111,
            },
            new Employee() {
                Name = "Delete Recursive",
                EmployeeId = 3333333,
            },
            new Employee() {
                Name = "InvalidUser2",
                EmployeeId = 2222222,
                UserId = "invalid user"
            },
            new Employee() {
                Name = "Delete Schedule Recursive",
                EmployeeId = 4444444,
            },
            new Employee() {
                Name = "Delete Request Recursive",
                EmployeeId = 5555555,
            }

        };
        public static List<Request> seedingRequests = new List<Request>() {
            new Request() {
                EmployeeId = 6439174,
                Description = "None",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() {
                EmployeeId = 6439174,
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() {
                EmployeeId = 1234567,
                Description = "None",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() {
                EmployeeId = 1234567,
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //invalid User
                EmployeeId = 7654321,
                UserId = "invalid user",
                Description = "None",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() { //invalid User
                EmployeeId = 7654321,
                UserId = "invalid user",
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Update
                EmployeeId = 1234566,
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Delete
                EmployeeId = 5555555,
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Delete Recursive
                EmployeeId = 3333333,
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Delete Recursive
                EmployeeId = 3333333,
                Description = "None",
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
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() {
                EmployeeId = 6439174,
                Week = 3,
                Year = 2022,
                Day = 2,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() {
                EmployeeId = 6439174,
                Week = 2,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString(),
            },
            new Schedule() {
                EmployeeId = 1234567,
                Week = 2,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() {
                EmployeeId = 1234567,
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //invalid User
                EmployeeId = 7654321,
                UserId = "invalid user",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //invalid User2
                EmployeeId = 2222222,
                UserId = "invalid user",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //Test Update
                EmployeeId = 1234566,
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //Test Delete
                EmployeeId = 4444444,
                Week = 1,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString(),
            },
            new Schedule() { //Test Delete Cascade
                EmployeeId = 3333333,
                Week = 1,
                Year = 2022,
                Day = 2,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //Test Delete Cascade
                EmployeeId = 3333333,
                Week = 1,
                Year = 2022,
                Day = 3,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
        };

        public static List<Shift> seedingShifts = new List<Shift>() {
            new Shift() {
                Name = "Shift 1",
                Start = "04:00",
                End = "12:00"
            },
            new Shift() {
                Name = "Shift 2",
                Start = "12:00",
                End = "20:00"
            }
        };

        public static List<RuleGroup> seedingRules = new List<RuleGroup>() {
            new RuleGroup() {
                Name = "Invalid User",
                Priority = 2,
                Status = true,
                UserId = "invalid user",
                Rules = "Day:Monday;Employee:1234567"
            },
            new RuleGroup() {
                Name = "Rule: Test",
                Priority = 0,
                Status = true,
                Rules = "Employee:6439174;day:not,wednesday;day:not,thursday;hours:<,40"
            },
            //new RuleGroup() {
            //    Name = "Rule: Test",
            //    Priority = 1,
            //    Status = true,
            //    Rules = "Employee:1234567;Employee:not,6439174;Shift:2"
            //},
            //new RuleGroup() {
            //    Name = "Rule: Test",
            //    Priority = 2,
            //    Status = true,
            //    Rules = "day:not,Wednesday;day:not,Thursday;Employee:6439174;shift:1;hours:<,40"
            //},
            //new RuleGroup() {
            //    Name = "Rule: Test",
            //    Priority = 3,
            //    Status = true,
            //    Rules = "Day:All;Shift:1;"
            //},
            //new RuleGroup() {
            //    Name = "Rule: Test",
            //    Priority = 4,
            //    Status = true,
            //    Rules = "Day:All;Shift:2;"
            //},
            //new RuleGroup() {
            //    Name = "Rule: Test",
            //    Priority = 5,
            //    Status = true,
            //    Rules = "day:wednesday;employee:6439174"
            //},
            //new RuleGroup() {
            //    Name = "Rule: Test",
            //    Priority = 6,
            //    Status = true,
            //    Rules = "Day:Tuesday;Employee:6439174;Employee:1234567;Shift:1"
            //},
        };

        public static void InitializeDbForTests(ScheduleDbContext db) {
            string userId = config["Test:userId"];
            seedingEmployees.ForEach(s => { if (s.UserId == null) s.UserId = userId; });
            seedingRequests.ForEach(s => { if (s.UserId == null) s.UserId = userId; });
            seedingRules.ForEach(s => { if (s.UserId == null) s.UserId = userId; });
            seedingSchedules.ForEach(s => { if (s.UserId == null) s.UserId = userId; });
            seedingShifts.ForEach(s => { if (s.UserId == null) s.UserId = userId; });

            db.Employees.AddRange(seedingEmployees);
            db.Requests.AddRange(seedingRequests);
            db.Schedules.AddRange(seedingSchedules);
            db.RuleGroups.AddRange(seedingRules);
            db.Shifts.AddRange(seedingShifts);
            db.SaveChanges();
        }

        public static void ReinitializeDbForTests(ScheduleDbContext db) {
            db.Employees.RemoveRange(db.Employees);
            db.Requests.RemoveRange(db.Requests);
            db.Schedules.RemoveRange(db.Schedules);
            db.RuleGroups.RemoveRange(db.RuleGroups);
            db.Shifts.RemoveRange(db.Shifts);

            InitializeDbForTests(db);
        }

        public static void SetClientToken(HttpClient client) {
            string clientId = config["Test:clientId"];
            string secret = config["Test:secret"];
            string domain = config["Test:authDomain"];
            var tokenClient = new RestClient(domain);
            var request = new RestRequest();
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json",
                "{\"client_id\":\"" + clientId + "\",\"client_secret\":\"" + secret + "\",\"audience\":\"https://scheduleapi20220831111508.azurewebsites.net/api\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            RestResponse response = tokenClient.Post(request);
            var str = response.Content;
            TokenResponse json = JsonConvert.DeserializeObject<TokenResponse>(str);

            client.DefaultRequestHeaders.Add("authorization", "bearer " + json.Access_token);
        }
    }
}