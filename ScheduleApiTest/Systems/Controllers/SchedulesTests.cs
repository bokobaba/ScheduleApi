using FluentAssertions;
using Newtonsoft.Json;
using ScheduleApi.Dtos.ScheduleDtos;
using ScheduleApi.Models;
using ScheduleApiTest.Fixtures;
using ScheduleApiTest.Helpers;
using System.Net;
using System.Net.Http.Json;

namespace ScheduleApiTest.Systems.Controllers {
    [Collection("ScheduleApi")]
    public class SchedulesTests : IClassFixture<CustomWebApplicationFactory<Program>> {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public SchedulesTests(CustomWebApplicationFactory<Program> factory) {
            _factory = factory;
            _client = _factory.CreateClient();

            Utilities.SetClientToken(_client);
        }

        [Fact]
        public async Task Schedules_Get_ReturnSuccess() {
            // Arrange
            string url = "/api/Schedules";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            string str = await response.Content.ReadAsStringAsync();
            List<GetScheduleDto> schedules = JsonConvert.DeserializeObject<List<GetScheduleDto>>(str);

            schedules.Should().NotBeNull();
            schedules.Should().HaveCount(c => c >= 5); //accounting for adds and deletes
            schedules.Should().Contain(s =>
                s.EmployeeId == Utilities.seedingEmployees[0].EmployeeId);
            schedules.Should().Contain(s =>
                s.EmployeeId == Utilities.seedingEmployees[1].EmployeeId);
        }

        [Fact]
        public async Task Schedules_EmployeeDaySchedule_ReturnSuccess() {
            string url = "/api/Schedules/EmployeeDaySchedule";
            EmployeeDayScheduleDto request = new EmployeeDayScheduleDto() {
                EmployeeId = 6439174,
                Year = 2022,
                Week = 2,
                Day = 1
            };
            //Act
            var response = await _client.PostAsJsonAsync(url, request);

            //Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            GetScheduleDto schedule = JsonConvert.DeserializeObject<GetScheduleDto>(str);
            Schedule? expected = Utilities.seedingSchedules[2];

            schedule.Should().NotBeNull();
            schedule.Should().Match<GetScheduleDto>(s =>
                s.EmployeeId == expected.EmployeeId &&
                s.Start == expected.Start &&
                s.End == expected.End &&
                s.Year == expected.Year &&
                s.Week == expected.Week &&
                s.Day == expected.Day);
        }

        [Fact]
        public async Task Schedules_EmployeeDaySchedule_ReturnError() {
            //Arrange
            string url = "/api/Schedules/EmployeeDaySchedule";
            EmployeeDayScheduleDto request1 = new EmployeeDayScheduleDto() {
                EmployeeId = 2222222,
                Year = 2022,
                Week = 2,
                Day = 1
            };
            EmployeeDayScheduleDto request2 = new EmployeeDayScheduleDto() {
                EmployeeId = 7654321,
                Year = 2022,
                Week = 2,
                Day = 1
            };
            EmployeeDayScheduleDto request3 = new EmployeeDayScheduleDto() {
                EmployeeId = 6439174,
                Year = 2022,
                Day = 1
            };
            //Act
            var response1 = await _client.PostAsJsonAsync(url, request1); //invalid employeeId
            var response2 = await _client.PostAsJsonAsync(url, request2); //invalid user
            var response3 = await _client.PostAsJsonAsync(url, request3); //invalid data

            //Assert
            response1.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response2.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Schedules_Create_ReturnSuccess() {
            //Arrange
            string url = "/api/Schedules";
            AddScheduleDto request = new AddScheduleDto() {
                EmployeeId = 6439174,
                Year = 2023,
                Week = 20,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddHours(1).ToShortTimeString()
            };

            //Act
            var response = await _client.PostAsJsonAsync(url, request);  //normal create employee

            //Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            GetScheduleDto req = JsonConvert.DeserializeObject<GetScheduleDto>(str);

            req.Should().NotBeNull();
            req.Should().Match<GetScheduleDto>(s =>
                s.EmployeeId == request.EmployeeId &&
                s.Start == request.Start &&
                s.End == request.End &&
                s.Year == request.Year &&
                s.Week == request.Week &&
                s.Day == request.Day);
        }

        [Fact]
        public async Task Schedules_Create_ReturnError() {
            //Arrange
            string url = "/api/Schedules";
            AddScheduleDto request1 = new AddScheduleDto() {
                EmployeeId = 1839250,
                Year = 2023,
                Week = 20,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddHours(1).ToShortTimeString()
            };
            AddScheduleDto request2 = new AddScheduleDto() {
                EmployeeId = 6439174,
                Year = 2023,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddHours(1).ToShortTimeString()
            };
            AddScheduleDto request3 = new AddScheduleDto() {
                EmployeeId = 7654321,
                Year = 2023,
                Week = 20,
                Day = 1,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddHours(1).ToShortTimeString()
            };

            //Act
            await _client.PostAsJsonAsync(url, request1);
            var response1 = await _client.PostAsJsonAsync(url, request1); //employee does not exist
            var response2 = await _client.PostAsJsonAsync(url, request2); //test missing data
            var response3 = await _client.PostAsJsonAsync(url, request3); //test invalid user

            //Assert
            response1.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response3.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Schedules_Update_ReturnSuccess() {
            //Arrange
            string url = "/api/Schedules";
            var request = new UpdateScheduleDto() {
                EmployeeId = 1234566,
                Start = DateTime.UtcNow.AddHours(3).ToShortTimeString(),
                End = DateTime.UtcNow.AddHours(5).ToShortTimeString(),
                Year = 2022,
                Week = 2,
                Day = 4,
            };

            //Act
            var response = await _client.PutAsJsonAsync(url, request);

            //Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            GetScheduleDto schedule = JsonConvert.DeserializeObject<GetScheduleDto>(str);

            schedule.Should().NotBeNull();
            schedule.Should().Match<GetScheduleDto>(s =>
                s.EmployeeId == request.EmployeeId &&
                s.Start == request.Start &&
                s.End == request.End &&
                s.Year == request.Year &&
                s.Week == request.Week &&
                s.Day == request.Day);
        }

        [Fact]
        public async Task Schedules_Update_ReturnError() {
            //Arrange
            string url = "/api/Schedules";
            var request2 = new UpdateScheduleDto() {
                EmployeeId = 7777777,
                Start = DateTime.UtcNow.AddHours(3).ToShortTimeString(),
                End = DateTime.UtcNow.AddHours(5).ToShortTimeString(),
                Year = 2022,
                Week = 2,
                Day = 4,
            };
            var request3 = new UpdateScheduleDto() {
                EmployeeId = 7654321,
                Week = 2,
                Year = 2022,
                Day = 4,
                Start = DateTime.UtcNow.ToShortTimeString(),
                End = DateTime.UtcNow.AddDays(1).ToShortTimeString()
            };
            var request4 = new UpdateScheduleDto() {
                EmployeeId = 1234567,
                Start = DateTime.UtcNow.AddHours(3).ToShortTimeString(),
                End = DateTime.UtcNow.AddHours(5).ToShortTimeString(),
                Year = 2022,
                Week = 2,
                Day = 5,
            };
            var request5 = new UpdateScheduleDto() {
                EmployeeId = 6439174,
                Start = DateTime.UtcNow.AddHours(3).ToShortTimeString(),
                End = DateTime.UtcNow.AddHours(5).ToShortTimeString(),
                Year = 2022,
                Week = 2,
            };

            //Act
            var response2 = await _client.PutAsJsonAsync(url, request2); //test inalid employeeId
            var response3 = await _client.PutAsJsonAsync(url, request3); //test invalid user
            var response4 = await _client.PutAsJsonAsync(url, request4); //test invalid date
            var response5 = await _client.PutAsJsonAsync(url, request5); //test invalid data

            //Assert
            response2.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response3.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response4.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response5.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Schedules_Delete_ReturnSuccess() {
            //Arrange
            string url1 = "/api/Schedules";
            string url2 = "/api/Employees/4444444";
            string url3 = "/api/Schedules/EmployeeDaySchedule";

            var request1 = new EmployeeDayScheduleDto() {
                EmployeeId = 4444444,
                Week = 1,
                Year = 2022,
                Day = 1,
            };
            var request3 = new EmployeeDayScheduleDto() {
                EmployeeId = 4444444,
                Week = 1,
                Year = 2022,
                Day = 3,
            };

            //Act
            HttpRequestMessage request = new HttpRequestMessage {
                Content = JsonContent.Create(request1),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url1, UriKind.Relative)
            };
            var response = await _client.SendAsync(request); //test delete single request
            var response2 = await _client.DeleteAsync(url2); //test delete employee should also delete requests
            var response3 = await _client.PostAsJsonAsync(url3, request3); // test if request is actually deleted

            //Assert
            response.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
            response3.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Schedules_Delete_ReturnError() {
            //Arrange
            string url1 = "/api/Schedules";
            string url2 = "/api/Schedules";

            var request1 = new EmployeeDayScheduleDto() {
                EmployeeId = 8888888,
                Week = 1,
                Year = 2022,
                Day = 1,
            };
            var request2 = new EmployeeDayScheduleDto() {
                EmployeeId = 7654321,
                Week = 2,
                Year = 2022,
                Day = 4,
            };

            //Act
            HttpRequestMessage request = new HttpRequestMessage {
                Content = JsonContent.Create(request1),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url1, UriKind.Relative)
            };
            var response = await _client.SendAsync(request); //test invalid employeeId
            request = new HttpRequestMessage {
                Content = JsonContent.Create(request2),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(url1, UriKind.Relative)
            };
            var response2 = await _client.SendAsync(request); //test invalid user

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response2.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        //[Fact]
        public async Task Schedules_Generate_Schedules_ReturnSuccess() {
            //Arrange
            string url = "/api/Schedules/GenerateSchedules";

            var request1 = new GenerateScheduleDto() {
                Year = 2022,
                Week = 1,
            };

            //Act
            HttpRequestMessage request = new HttpRequestMessage {
                Content = JsonContent.Create(request1),
                Method = HttpMethod.Post,
                RequestUri = new Uri(url, UriKind.Relative)
            };
            var response = await _client.SendAsync(request);

            //Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
