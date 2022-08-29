using ScheduleApi.Dtos.UserDtos;

namespace ScheduleApi.Services.AuthService {
    public interface IAuthService {
        public Task<Guid> Register(UserRegisterDto userRegister);
        public Task<string> Login(UserLoginDto userLogin);
        public Task<bool> UserExists(string email);
    }
}
