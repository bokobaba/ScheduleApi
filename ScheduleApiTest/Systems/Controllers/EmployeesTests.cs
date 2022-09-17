using FluentAssertions;
using Newtonsoft.Json;
using RestSharp;
using ScheduleApi.Dtos.EmployeeDtos;
using ScheduleApi.Models;
using ScheduleApiTest.Fixtures;
using ScheduleApiTest.Helpers;
using ScheduleApiTest.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ScheduleApiTest.Systems.Controllers {
    public class EmployeesTests : IClassFixture<CustomWebApplicationFactory<Program>> {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public EmployeesTests(CustomWebApplicationFactory<Program> factory) {
            _factory = factory;
            _client = _factory.CreateClient();

            Utilities.SetClientToken(_client);
}

        [Fact]
        public async Task Employees_Get_ReturnSuccess() {
            // Arrange
            string url = "/api/Employees";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            string str = await response.Content.ReadAsStringAsync();
            List<GetEmployeeDto> employees = JsonConvert.DeserializeObject<List<GetEmployeeDto>>(str);

            employees.Should().NotBeNull();
            //employees.Should().HaveCount(2);
            employees.Should().Contain(e =>
                e.EmployeeId == Utilities.seedingEmployees[0].EmployeeId &&
                e.Name == Utilities.seedingEmployees[0].Name);
            employees.Should().Contain(e =>
                e.EmployeeId == Utilities.seedingEmployees[1].EmployeeId &&
                e.Name == Utilities.seedingEmployees[1].Name);
        }

        [Theory]
        [InlineData(6439174)]
        [InlineData(1234567)]
        public async Task Employees_GetById_ReturnSuccess(int id) {
            string url = "/api/Employees/" + id;
            //Act
            var response = await _client.GetAsync(url);

            //Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            GetEmployeeDto employee = JsonConvert.DeserializeObject<GetEmployeeDto>(str);
            Employee? expected = Utilities.seedingEmployees.FirstOrDefault(e => e.EmployeeId == id);

            employee.Should().NotBeNull();
            employee.Should().Match<GetEmployeeDto>(e =>
                e.EmployeeId == expected.EmployeeId &&
                e.Name == expected.Name);
        }

        [Theory]
        [InlineData(7654321)]
        [InlineData(1234568)]
        public async Task Employees_GetById_ReturnError(int id) {
            //Arrange
            string url = "/api/Employees/" + id;

            //Act
            var response = await _client.GetAsync(url);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Employees_Create_ReturnSuccess() {
            //Arrange
            string url = "/api/Employees";
            AddEmployeeDto request = new AddEmployeeDto() {
                EmployeeId = 9876543,
                Name = "Jenny",
            };
            AddEmployeeDto request2 = new AddEmployeeDto() {
                EmployeeId = 2222222,
                Name = "Jeff"
            };

            //Act
            var response = await _client.PostAsJsonAsync(url, request);  //normal create employee
            var response2 = await _client.PostAsJsonAsync(url, request2); //id already exists under different user

            //Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            GetEmployeeDto employee = JsonConvert.DeserializeObject<GetEmployeeDto>(str);

            employee.Should().NotBeNull();
            employee.Should().Match<GetEmployeeDto>(e =>
                e.EmployeeId == request.EmployeeId &&
                e.Name == request.Name);

            response2.EnsureSuccessStatusCode();

            string str2 = await response2.Content.ReadAsStringAsync();
            GetEmployeeDto employee2 = JsonConvert.DeserializeObject<GetEmployeeDto>(str2);

            employee2.Should().NotBeNull();
            employee2.Should().Match<GetEmployeeDto>(e =>
                e.EmployeeId == request2.EmployeeId &&
                e.Name == request2.Name);
        }

        [Fact]
        public async Task Employees_Create_ReturnError() {
            //Arrange
            string url = "/api/Employees";
            AddEmployeeDto request1 = new AddEmployeeDto() {
                EmployeeId = 6439174,
                Name = "Dupe",
            };
            AddEmployeeDto request2 = new AddEmployeeDto() {
                EmployeeId = 9876541,
            };

            //Act
            await _client.PostAsJsonAsync(url, request1);   
            var response1 = await _client.PostAsJsonAsync(url, request1); //test id already exists
            var response2 = await _client.PostAsJsonAsync(url, request2); //test missing data

            //Assert
            response1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Employees_Update_ReturnSuccess() {
            //Arrange
            string url = "/api/Employees/1234566";
            var request = new UpdateEmployeeDto() {
                EmployeeId = 1234566,
                Name = "TestUpdate"
            };

            //Act
            var response = await _client.PutAsJsonAsync(url, request);

            //Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            GetEmployeeDto employee = JsonConvert.DeserializeObject<GetEmployeeDto>(str);

            employee.Should().NotBeNull();
            employee.Should().Match<GetEmployeeDto>(e =>
                e.EmployeeId == request.EmployeeId &&
                e.Name == request.Name);
        }

        [Fact]
        public async Task Employees_Update_ReturnError() {
            //Arrange
            string url = "/api/Employees/6439174";
            string url2 = "/api/Employees/6439171";
            string url3 = "/api/Employees/7654321";
            var request = new UpdateEmployeeDto() {
                EmployeeId = 6439174,
                Name = "ParkerTest"
            };
            var request2 = new UpdateEmployeeDto() {
                EmployeeId = 6439171,
                Name = "ParkerTest"
            };
            var request3 = new UpdateEmployeeDto() {
                EmployeeId = 7654321,
                Name = "Invalid User"
            };

                //Act
            var response1 = await _client.PutAsJsonAsync(url2, request);  //test inconsistent id
            var response2 = await _client.PutAsJsonAsync(url2, request2); //test inalid id
            var response3 = await _client.PutAsJsonAsync(url2, request3); //test invalid user

            //Assert
            response1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response2.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Employees_Delete_ReturnSuccess() {
            //Arrange
            string url = "/api/Employees/1111111";

            //Act
            var response = await _client.DeleteAsync(url);

            //Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Employees_Delete_ReturnError() {
            //Arrange
            string url = "/api/Employees/2222222";
            string url2 = "/api/Employees/7654321";

            //Act
            var response = await _client.DeleteAsync(url); //test invalid id
            var response2 = await _client.DeleteAsync(url2); //test invalid user

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response2.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
