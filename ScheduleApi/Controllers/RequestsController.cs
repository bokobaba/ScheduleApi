using Microsoft.AspNetCore.Mvc;
using ScheduleApi.Models;
using Microsoft.AspNetCore.Authorization;
using ScheduleApi.Services.RequestService;
using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Dtos.EmployeeDtos;
using ScheduleApi.Exceptions;

namespace ScheduleApi.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase {
        private readonly IRequestService _service;

        public RequestsController(IRequestService service) {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<GetEmployeeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() {
            IEnumerable<GetRequestDto>? response = await _service.GetAllRequests();

            return response == null ? NoContent() : Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Request), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id) {
            return Ok(await _service.GetReqeustById(id));
        }

        [HttpGet("get-employee-requests/{id}")]
        [ProducesResponseType(typeof(IEnumerable<Request>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmployeeId(int employeeId) {
            IEnumerable<GetRequestDto>? response = await _service.GetReqeustsByEmployeeId(employeeId);

            return response == null ? NoContent() : Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] AddRequestDto request) {
            GetRequestDto response = await _service.AddRequest(request);

            return CreatedAtAction(nameof(GetById),
                new {
                    id = response.ID,
                },
                request);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRequestDto request) {
            if (id != request.ID)
                throw new AppException(
                    string.Format("id = {0} does not match that of the employee to update id = {1}",
                    id, request.ID));

            await _service.UpdateRequest(request);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id) {
            await _service.DeleteRequest(id);

            return NoContent();
        }
    }
}
