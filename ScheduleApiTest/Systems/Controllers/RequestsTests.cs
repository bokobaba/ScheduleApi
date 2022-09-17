using FluentAssertions;
using Newtonsoft.Json;
using RestSharp;
using ScheduleApi.Dtos.EmployeeDtos;
using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Models;
using ScheduleApiTest.Fixtures;
using ScheduleApiTest.Helpers;
using ScheduleApiTest.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ScheduleApiTest.Systems.Controllers {
    public class RequestsTests : IClassFixture<CustomWebApplicationFactory<Program>> {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public RequestsTests(CustomWebApplicationFactory<Program> factory) {
            _factory = factory;
            _client = _factory.CreateClient();

            Utilities.SetClientToken(_client);
        }

        [Fact]
        public async Task Requests_Get_ReturnSuccess() {
            // Arrange
            string url = "/api/Requests";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            string str = await response.Content.ReadAsStringAsync();
            List<GetRequestDto> requests = JsonConvert.DeserializeObject<List<GetRequestDto>>(str);

            requests.Should().NotBeNull();
            requests.Should().HaveCount(c => c >= 4); //accounting for adds and deletes
            requests.Should().Contain(e =>
                e.EmployeeId == Utilities.seedingEmployees[0].EmployeeId);
            requests.Should().Contain(e =>
                e.EmployeeId == Utilities.seedingEmployees[1].EmployeeId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public async Task Requests_GetById_ReturnSuccess(int id) {
            string url = "/api/Requests/" + id;
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            GetRequestDto request = JsonConvert.DeserializeObject<GetRequestDto>(str);
            Request? expected = Utilities.seedingRequests.FirstOrDefault(r => r.ID == id);

            request.Should().NotBeNull();
            request.Should().Match<GetRequestDto>(r =>
                r.EmployeeId == expected.EmployeeId &&
                r.Start == expected.Start &&
                r.End == expected.End);
        }

        [Theory]
        [InlineData(100)] //invalid id
        [InlineData(5)] //invlalid user
        [InlineData(6)] //invalid user
        public async Task Requests_GetById_ReturnError(int id) {
            //Arrange
            string url = "/api/Requests/" + id;

            //Act
            var response = await _client.GetAsync(url);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Requests_Create_ReturnSuccess() {
            //Arrange
            string url = "/api/Requests";
            AddRequestDto request = new AddRequestDto() {
                EmployeeId = 6439174,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(1)
            };

            //Act
            var response = await _client.PostAsJsonAsync(url, request);  //normal create employee

            //Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            GetRequestDto req = JsonConvert.DeserializeObject<GetRequestDto>(str);

            req.Should().NotBeNull();
            req.Should().Match<GetRequestDto>(r => 
                r.EmployeeId == request.EmployeeId &&
                r.Start == request.Start &&
                r.End == request.End);
        }

        [Fact]
        public async Task Requests_Create_ReturnError() {
            //Arrange
            string url = "/api/Requests";
            AddRequestDto request1 = new AddRequestDto() {
                EmployeeId = 5555555,
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddHours(1)
            };
            AddRequestDto request2 = new AddRequestDto() {
                EmployeeId = 6439174,
                Start = DateTime.UtcNow,
            };
            AddRequestDto request3 = new AddRequestDto() {
                EmployeeId = 7654321,
                Start = DateTime.UtcNow,
                End= DateTime.UtcNow.AddHours(2)
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
        public async Task Requests_Update_ReturnSuccess() {
            //Arrange
            string url = "/api/Requests/7";
            var request = new UpdateRequestDto() {
                ID = 7,
                EmployeeId = 1234566,
                Start = DateTime.UtcNow.AddHours(3),
                End = DateTime.UtcNow.AddHours(5),
            };

            //Act
            var response = await _client.PutAsJsonAsync(url, request);

            //Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            GetRequestDto req = JsonConvert.DeserializeObject<GetRequestDto>(str);

            req.Should().NotBeNull();
            req.Should().Match<GetRequestDto>(r =>
                r.EmployeeId == request.EmployeeId &&
                r.Start == request.Start &&
                r.End == request.End);
        }

        [Fact]
        public async Task Requests_Update_ReturnError() {
            //Arrange
            string url1 = "/api/Requests/2";
            string url2 = "/api/Requests/10";
            string url3 = "/api/Requests/5";
            string url4 = "/api/Requests/1";
            var request1 = new UpdateRequestDto() {
                ID = 1,
                EmployeeId = 6439174,
                Start = DateTime.UtcNow.AddHours(6),
                End = DateTime.UtcNow.AddHours(7)
            };
            var request2 = new UpdateRequestDto() {
                ID = 10,
                EmployeeId = 6439174,
                Start = DateTime.UtcNow.AddHours(6),
                End = DateTime.UtcNow.AddHours(7)
            };
            var request3 = new UpdateRequestDto() {
                ID = 5,
                EmployeeId = 7654321,
                Start = DateTime.UtcNow.AddHours(6),
                End = DateTime.UtcNow.AddHours(7)
            };
            var request4 = new UpdateRequestDto() {
                ID = 1,
                EmployeeId = 1234567,
                Start = DateTime.UtcNow.AddHours(6),
                End = DateTime.UtcNow.AddHours(7)
            };
            var request5 = new UpdateRequestDto() {
                ID = 1,
                EmployeeId = 6439174,
                End = DateTime.UtcNow.AddHours(7)
            };

            //Act
            var response1 = await _client.PutAsJsonAsync(url1, request1); //test inconsistent id
            var response2 = await _client.PutAsJsonAsync(url2, request2); //test inalid id
            var response3 = await _client.PutAsJsonAsync(url3, request3); //test invalid user
            var response4 = await _client.PutAsJsonAsync(url4, request4); //test inconsistent employeeId
            var response5 = await _client.PutAsJsonAsync(url4, request5); //test invalid data

            //Assert
            response1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response3.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response4.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response5.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Requests_Delete_ReturnSuccess() {
            //Arrange
            string url1 = "/api/Requests/8";
            string url2 = "/api/Employees/1111111";
            string url3 = "/api/Requests/9";
            string url4 = "/api/Requests/10";

            //Act
            var response = await _client.DeleteAsync(url1); //test delete single request
            var response2 = await _client.DeleteAsync(url2); //test delete employee should also delete requests
            var response3 = await _client.GetAsync(url3); // test if request is actually deleted
            var response4 = await _client.GetAsync(url4); // test if request is actually deleted

            //Assert
            response.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
            response3.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response4.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Requests_Delete_ReturnError() {
            //Arrange
            string url = "/api/Requests/30";
            string url2 = "/api/Requests/5";

            //Act
            var response = await _client.DeleteAsync(url); //test invalid id
            var response2 = await _client.DeleteAsync(url2); //test invalid user

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response2.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
