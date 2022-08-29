using ScheduleApi.Dtos.UserDtos;

namespace ScheduleApi.Services.AuthService {
    public interface IAuthService {
        public Task<RegisterResponseDto> Register(UserRegisterDto userRegister);
        public Task<AuthResponseDto> Login(UserLoginDto userLogin);
        public Task<bool> UserExists(string email);
        public Task<AuthResponseDto> RefreshToken(RefreshTokenRequest request);
        public Task<bool> IsTokenIpAddressValid(string accessToken, string ipAddress);
    }
}
