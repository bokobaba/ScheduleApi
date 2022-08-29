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

        public AuthService(ScheduleDbContext context, IConfiguration config) {
            _context = context;
            _config = config;
        }

        public async Task<string> Login(UserLoginDto userLogin) {
            User? user = await _context.Users.FirstOrDefaultAsync(
                u => u.EmailAddress.ToLower() == userLogin.EmailAddress.ToLower());

            if (user == null) {
                throw new KeyNotFoundException(
                    string.Format("user with email = {0} not found", userLogin.EmailAddress));
            }

            if (!VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt)) {
                throw new AppException("invalid password");
            }

            return GenerateToken(user);
        }

        public async Task<Guid> Register(UserRegisterDto userRegister) {
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

            return user.ID;
        }

        public async Task<bool> UserExists(string email) {
            if (await _context.Users.AnyAsync(u => u.EmailAddress.ToLower() == email.ToLower())) {
                return true;
            }
            return false;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash,
            out byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
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
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserRefreshToken GenerateRefreshToken() {
            var refreshToken = new UserRefreshToken() {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(UserRefreshToken newRefreshToken) {
            var cookieOptions = new CookieOptions {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };

        }
    }
}
