using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Dtos.ShiftDtos;
using ScheduleApi.Services.ShiftService;

namespace ScheduleApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftsController : ControllerBase {
        private readonly IShiftService _service;

        public ShiftsController(IShiftService service) {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetRequestDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() {
            IEnumerable<GetShiftDto> response = await _service.GetAllShifts();

            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id) {
            GetShiftDto response = await _service.GetById(id);

            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(AddShiftDto request) {
            GetShiftDto response = await _service.CreateShifts(request);

            return CreatedAtAction(nameof(GetById),
                new {
                    id = response.Id,
                },
                response);
        }

        [HttpPut]
        [ProducesResponseType(typeof(GetRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(UpdateShiftDto request) {
            GetShiftDto response = await _service.UpdateShift(request);

            return Ok(response);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id) {
            await _service.DeleteShift(id);

            return NoContent();
        }
    }
}
