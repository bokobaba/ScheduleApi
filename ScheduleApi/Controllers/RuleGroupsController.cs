using Microsoft.AspNetCore.Mvc;
using ScheduleApi.Dtos.RuleGroupDtos;
using ScheduleApi.Services.RuleGroupService;

namespace ScheduleApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RuleGroupsController : ControllerBase {
        private IRuleGroupService _service;

        public RuleGroupsController(IRuleGroupService service) {
            _service = service;
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetRuleGroupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() {
            IEnumerable<GetRuleGroupDto> response = await _service.GetRuleGroups();

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<GetRuleGroupDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveRules(SaveRuleGroupsDto request) {
            IEnumerable<GetRuleGroupDto> response = await _service.SaveRuleGroups(request);

            return Ok(response);
        }
    }
}
