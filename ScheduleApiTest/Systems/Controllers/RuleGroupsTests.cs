using FluentAssertions;
using Newtonsoft.Json;
using ScheduleApi.Dtos.RuleGroupDtos;
using ScheduleApiTest.Fixtures;
using ScheduleApiTest.Helpers;
using System.Net.Http.Json;

namespace ScheduleApiTest.Systems.Controllers {
    [Collection("ScheduleApi")]
    public class RuleGroupsTests : IClassFixture<CustomWebApplicationFactory<Program>> {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public RuleGroupsTests(CustomWebApplicationFactory<Program> factory) {
            _factory = factory;
            _client = _factory.CreateClient();

            Utilities.SetClientToken(_client);
        }

        [Fact]
        public async Task RuleGroups_Get_ReturnSuccess() {
            // Arrange
            string url = "/api/RuleGroups";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299

            string str = await response.Content.ReadAsStringAsync();
            List<GetRuleGroupDto> rules = JsonConvert.DeserializeObject<List<GetRuleGroupDto>>(str);

            rules.Should().NotBeNull();
            rules.Should().HaveCount(c => c == Utilities.seedingRules.Count - 1); //accounting for adds and deletes
        }

        [Fact]
        public async Task RuleGroups_SaveRuleGroups_ReturnSuccess() {
            // Arrange
            string url = "/api/RuleGroups";
            SaveRuleGroupsDto request = new SaveRuleGroupsDto() {
                Rules = new List<SaveRuleGroupDto>() {
                    new SaveRuleGroupDto() {
                        Name = "New Rule: 1",
                        Priority = 0,
                        Status = true,
                        Rules = "Day:Monday;AND,Employee:Josh"
                     },
                    new SaveRuleGroupDto() {
                        Name = "New Rule: 2",
                        Priority = 0,
                        Status = true,
                        Rules = "Day:Monday;AND,Employee:Josh"
                     },
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync(url, request);

            // Assert
            response.EnsureSuccessStatusCode();

            string str = await response.Content.ReadAsStringAsync();
            List<GetRuleGroupDto> rules = JsonConvert.DeserializeObject<List<GetRuleGroupDto>>(str);

            rules.Should().NotBeNull();
            rules.Should().HaveCount(c => c == 2);

            rules.Should().Contain(c => c.Name == request.Rules[0].Name);
            rules.Should().Contain(c => c.Name == request.Rules[1].Name);
        }

    }
}
