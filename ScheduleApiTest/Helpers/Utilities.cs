﻿

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using ScheduleApi.Data;
using ScheduleApi.Models;
using ScheduleApiTest.Models;

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
                Name = "Test Update",
                EmployeeId = 1234566,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
            },
            new Employee() {
                Name = "Delete",
                EmployeeId = 1111111,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
            },
            new Employee() {
                Name = "Delete Recursive",
                EmployeeId = 3333333,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
            },
            new Employee() {
                Name = "InvalidUser2",
                EmployeeId = 2222222,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients"
            },
            new Employee() {
                Name = "Delete Schedule Recursive",
                EmployeeId = 4444444,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients"
            },
            new Employee() {
                Name = "Delete Request Recursive",
                EmployeeId = 5555555,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients"
            }

        };
        public static List<Request> seedingRequests = new List<Request>() {
            new Request() {
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() {
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() {
                EmployeeId = 1234567,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() {
                EmployeeId = 1234567,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //invalid User
                EmployeeId = 7654321,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() { //invalid User
                EmployeeId = 7654321,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Update
                EmployeeId = 1234566,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Delete
                EmployeeId = 5555555,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Delete Recursive
                EmployeeId = 3333333,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Delete Recursive
                EmployeeId = 3333333,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Description = "None",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
        };
        public static List<Schedule> seedingSchedules = new List<Schedule>() {
            new Schedule() {
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 3,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() {
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 3,
                Year = 2022,
                Day = 2,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() {
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString(),
            },
            new Schedule() {
                EmployeeId = 1234567,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() {
                EmployeeId = 1234567,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //invalid User
                EmployeeId = 7654321,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //invalid User2
                EmployeeId = 2222222,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //Test Update
                EmployeeId = 1234566,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //Test Delete
                EmployeeId = 4444444,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 1,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString(),
            },
            new Schedule() { //Test Delete Cascade
                EmployeeId = 3333333,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 1,
                Year = 2022,
                Day = 2,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            },
            new Schedule() { //Test Delete Cascade
                EmployeeId = 3333333,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
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
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Start = "04:00",
                End = "12:00"
            },
            new Shift() {
                Name = "Shift 2",
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Start = "09:00",
                End = "17:00"
            }
        };

        public static List<RuleGroup> seedingRules = new List<RuleGroup>() {
            new RuleGroup() {
                Name = "Rule: 1",
                Priority = 2,
                Status = true,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Rules = "Day:Monday;Employee:1234567"
            },
            new RuleGroup() {
                Name = "Rule: 2",
                Priority = 1,
                Status = true,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Rules = "Day:Tuesday;Employee:6439174;Employee:1234567"
            },
            new RuleGroup() {
                Name = "Invalid User",
                Priority = 2,
                Status = true,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
                Rules = "Day:Monday;Employee:1234567"
            },
            new RuleGroup() {
                Name = "Rule: 1",
                Priority = 0,
                Status = true,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Rules = "day:not,Wednesday;day:not,Thursday;Employee:6439174;hours:=,40;Day:All;Shift:0"
            },
        };

        public static void InitializeDbForTests(ScheduleDbContext db) {
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

        public static void SetClientToken(HttpClient client, IConfigurationRoot config) {
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