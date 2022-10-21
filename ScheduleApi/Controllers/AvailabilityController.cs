using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleApi.Dtos.AvailabilityDtos;
using ScheduleApi.Models;
using ScheduleApi.Services.AvailabilityService;

namespace ScheduleApi.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase {
        private readonly IAvailabilityService _service;

        public AvailabilityController(IAvailabilityService service) {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetAvailabilityDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() {
            IEnumerable<GetAvailabilityDto> response = await _service.GetAllAvailabilities();

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof (IEnumerable<GetAvailabilityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id) {
            IEnumerable<GetAvailabilityDto> response = await _service.GetAvailabilityForEmployee(id);

            return Ok(response);
        }

        [HttpPost("{id}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(int id) {
            IEnumerable<GetAvailabilityDto> response = await _service.CreateAvailability(id);

            return CreatedAtAction(nameof(GetById),
                new {
                    id = id,
                },
                response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(UpdateEmployeeAvailabilityDto request) {
            IEnumerable<GetAvailabilityDto> response = await _service.UpdateAvailability(request);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id) {
            await _service.DeleteAvailability(id);

            return NoContent();
        }
    }
}
