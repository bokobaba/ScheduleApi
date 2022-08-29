using Microsoft.AspNetCore.Mvc;
using ScheduleApi.Dtos.UserDtos;
using ScheduleApi.Services.AuthService;

namespace ScheduleApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase {
        private readonly IAuthService _service;

        public AccountController(IAuthService service) {
            _service = service;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> Register(UserRegisterDto request) {
            Guid response = await _service.Register(request);

            return Ok(response);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> Login(UserLoginDto request) {
            string response = await _service.Login(request);

            return Ok(response);
        }

    }
}
