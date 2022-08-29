using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScheduleApi.Data;
using ScheduleApi.Dtos.UserDtos;
using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ScheduleApi.Services.AuthService {
    public class AuthService : IAuthService {
        private readonly ScheduleDbContext _context;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthService(ScheduleDbContext context, IConfiguration config,
            IHttpContextAccessor contextAccessor) {
            _context = context;
            _config = config;
            _contextAccessor = contextAccessor;
        }

        public async Task<AuthResponseDto> Login(UserLoginDto userLogin) {
            User? user = await _context.Users.FirstOrDefaultAsync(
                u => u.EmailAddress.ToLower() == userLogin.EmailAddress.ToLower());

            if (user == null) {
                throw new KeyNotFoundException(
                    string.Format("user with email = {0} not found", userLogin.EmailAddress));
            }

            if (!VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt)) {
                throw new AppException("invalid password");
            }

            string refreshToken = GenerateRefreshToken();
            string token = GenerateToken(user);
            await SaveRefreshToken(user, refreshToken, token);

            return new AuthResponseDto {
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<RegisterResponseDto> Register(UserRegisterDto userRegister) {
            CreatePasswordHash(userRegister.Password, out byte[] passwordHash, out byte[] passwordSalt);

            if (await UserExists(userRegister.EmailAddress)) {
                throw new AppException("User already exists");
            }

            var user = new User() {
                Username = userRegister.Username,
                EmailAddress = userRegister.EmailAddress,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new RegisterResponseDto {
                Username = userRegister.Username,
                EmailAddress = userRegister.EmailAddress
            };
        }

        public async Task<bool> UserExists(string email) {
            if (await _context.Users.AnyAsync(u => u.EmailAddress.ToLower() == email.ToLower())) {
                return true;
            }
            return false;
        }

        public async Task<AuthResponseDto> RefreshToken(RefreshTokenRequest request) {
            User user = await GetUserFromRefreshToken(request);

            var refreshToken = GenerateRefreshToken();
            var accessToken = GenerateToken(user);

            await SaveRefreshToken(user, refreshToken, accessToken);

            return new AuthResponseDto {
                RefreshToken = refreshToken,
                Token = accessToken
            };
        }

        private async Task SaveRefreshToken(User user, string refreshToken, string token) {
            var userRefreshToken = new UserRefreshToken() {
                Token = token,
                RefreshToken = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                IpAddress = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                IsInvalidated = false,
                UserId = user.ID,
            };

            _context.RefreshTokens.Add(userRefreshToken);
            await _context.SaveChangesAsync();
        }

        private async Task<User> GetUserFromRefreshToken(RefreshTokenRequest request) {
            var token = GetJwtToken(request.ExpiredToken);

            UserRefreshToken? userRefreshToken = _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefault(
                r => r.IsInvalidated == false && 
                r.Token == request.ExpiredToken &&
                r.RefreshToken == request.RefreshToken && 
                r.IpAddress == _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());

            if (userRefreshToken.User == null)
                throw new AppException("Invalid token Details");

            ValidateDetails(token, userRefreshToken);

            userRefreshToken.IsInvalidated = true;
            await _context.SaveChangesAsync();

            return userRefreshToken.User;
        }

        private void ValidateDetails(JwtSecurityToken token, UserRefreshToken? userRefreshToken) {
            if (userRefreshToken == null)
                throw new AppException("Invalid token Details");
            if (token.ValidTo > DateTime.UtcNow)
                throw new AppException("Token not expired");
            if (!userRefreshToken.IsActive)
                throw new AppException("Refresh Token Expired");

        }

        public async Task<bool> IsTokenIpAddressValid(string accessToken, string ipAddress) {
            bool isValid = _context.RefreshTokens.FirstOrDefaultAsync(
                r => r.Token == accessToken &&
                r.IpAddress == ipAddress) != null;
            return await Task.FromResult(isValid);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash,
            out byte[] passwordSalt) {
            using (var hmac = new HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            using (var hmac = new HMACSHA512(passwordSalt)) {
                byte[] computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string GenerateToken(User user) {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken() {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private JwtSecurityToken GetJwtToken(string expiredToken) {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(expiredToken);
        }
    }
}
