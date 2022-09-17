

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
                Name = "InvalidUser2",
                EmployeeId = 2222222,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients"
            }

        };
        public static List<Request> seedingRequests = new List<Request>() {
            new Request() {
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() {
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() {
                EmployeeId = 1234567,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() {
                EmployeeId = 1234567,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //invalid User
                EmployeeId = 7654321,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(3)
            },
            new Request() { //invalid User
                EmployeeId = 7654321,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Update
                EmployeeId = 1234566,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Delete
                EmployeeId = 1111111,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Delete Cascade
                EmployeeId = 1111111,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Start = DateTime.UtcNow.AddDays(2),
                End = DateTime.UtcNow.AddDays(2).AddHours(3)
            },
            new Request() { //Test Delete Cascade
                EmployeeId = 1111111,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
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
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() {
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 3,
                Year = 2022,
                Day = 2,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() {
                EmployeeId = 6439174,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() {
                EmployeeId = 1234567,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() {
                EmployeeId = 1234567,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() { //invalid User
                EmployeeId = 7654321,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() { //invalid User2
                EmployeeId = 2222222,
                UserId = "a8XPDboKMpGRlFw1xUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() { //Test Update
                EmployeeId = 1234566,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() { //Test Delete
                EmployeeId = 1111111,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 1,
                Year = 2022,
                Day = 1,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() { //Test Delete Cascade
                EmployeeId = 1111111,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 1,
                Year = 2022,
                Day = 2,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(1)
            },
            new Schedule() { //Test Delete Cascade
                EmployeeId = 1111111,
                UserId = "a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH@clients",
                Week = 1,
                Year = 2022,
                Day = 3,
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

        public static void SetClientToken(HttpClient client) {
            var tokenClient = new RestClient("https://dev-0yfu4fcd.us.auth0.com/oauth/token");
            var request = new RestRequest();
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json",
                "{\"client_id\":\"a8XPDboKMpGRlF1wxUx4Xzrz94OrF5qH\",\"client_secret\":\"BV4EIi5rqWkUcb8dFwO720dRMH2VFDb5ttL4cP18yvJIJ24zUFqwDE0ZfoGo_Id2\",\"audience\":\"https://scheduleapi20220831111508.azurewebsites.net/api\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            RestResponse response = tokenClient.Post(request);
            var str = response.Content;
            TokenResponse json = JsonConvert.DeserializeObject<TokenResponse>(str);

            client.DefaultRequestHeaders.Add("authorization", "bearer " + json.Access_token);
        }
    }
}