using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleApi.Dtos.EmployeeDtos;
using ScheduleApi.Exceptions;
using ScheduleApi.Services.EmployeeService;

namespace ScheduleApi.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase {
        private readonly IEmployeeService _service;

        public EmployeesController(IEmployeeService service) {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetEmployeeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() {
            IEnumerable<GetEmployeeDto> response = await _service.GetAllEmployees();
            return Ok(response);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetEmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id) {
            return Ok(await _service.GetEmployeeById(id));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(AddEmployeeDto employee) {
            GetEmployeeDto response = await _service.AddEmployee(employee);

            return CreatedAtAction(nameof(GetById),
                new {
                    id = response.EmployeeId,
                },
                employee);

        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(GetEmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, UpdateEmployeeDto employee) {
            if (id != employee.EmployeeId) {
                throw new AppException(
                    string.Format("id = {0} does not match that of the employee to update id = {1}",
                    id, employee.EmployeeId));
            }

            await _service.UpdateEmployee(employee);

            return Ok(employee);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id) {
            await _service.DeleteEmployee(id);

            return NoContent();
        }
    }
}
