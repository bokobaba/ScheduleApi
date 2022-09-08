using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Dtos.ScheduleDtos;
using ScheduleApi.Exceptions;
using ScheduleApi.Services.ScheduleService;

namespace ScheduleApi.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase {
        private readonly IScheduleService _service;

        public SchedulesController(IScheduleService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            IEnumerable<GetScheduleDto>? response = await _service.GetAllSchedules();

            return response == null ? NoContent() : Ok(response);
        }

        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(GetScheduleDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetById(int id) {
        //    return Ok(await _service.GetScheduleById(id));
        //}

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(IEnumerable<GetScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EmployeeDaySchedule([FromBody] EmployeeDayScheduleDto request) {
            GetScheduleDto response = await _service.GetScheduleByDayEmployee(request);

            return Ok(response);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(IEnumerable<GetScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> WeeklySchedule(WeeklyScheduleDto request) {
            IEnumerable<GetScheduleDto> schedules = await _service.GetSchedulesByWeek(request);

            return Ok(schedules);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(IEnumerable<GetScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EmployeeWeeklySchedule(EmployeeWeeklyScheduleDto request) {
            IEnumerable<GetScheduleDto> schedules = await _service.GetSchedulesByWeekEmployee(request);

            return Ok(schedules);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(IEnumerable<GetScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SaveWeeklySchedule(SaveWeeklyScheduleDto request) {
            IEnumerable<GetScheduleDto> schedules = await _service.SaveWeeklySchedule(request);

            return Ok(schedules);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(AddScheduleDto request) {
            GetScheduleDto response = await _service.AddSchedule(request);

            return CreatedAtAction(nameof(EmployeeDaySchedule), null,
                response);
        }

        [HttpPut]
        [ProducesResponseType(typeof(GetRequestDto),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSchedule(UpdateScheduleDto request) {
            GetScheduleDto response = await _service.UpdateSchedule(request);

            return Ok(response);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(EmployeeDayScheduleDto request) {
            await _service.DeleteSchedule(request);

            return NoContent();
        }
    }
}
