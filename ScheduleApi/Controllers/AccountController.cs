using Microsoft.AspNetCore.Mvc;
using ScheduleApi.Dtos.UserDtos;
using ScheduleApi.Services.AuthService;
using System.IdentityModel.Tokens.Jwt;

namespace ScheduleApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase {
        private readonly IAuthService _service;

        internal AccountController(IAuthService service) {
            _service = service;
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        internal async Task<ActionResult<RegisterResponseDto>> Register(UserRegisterDto request) {
            RegisterResponseDto response = await _service.Register(request);

            return Ok(response);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        internal async Task<ActionResult<AuthResponseDto>> Login(UserLoginDto request) {
            AuthResponseDto response = await _service.Login(request);

            return Ok(response);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        internal async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequest request) {
            AuthResponseDto response = await _service.RefreshToken(request);

            return Ok(response);
        }
    }
}
