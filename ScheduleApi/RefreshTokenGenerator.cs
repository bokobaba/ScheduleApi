using ScheduleApi.Data;
using System.Security.Cryptography;

namespace ScheduleApi {
    public class RefreshTokenGenerator : IRefreshTokenGenerator {

        private readonly ScheduleDbContext _context;

        public RefreshTokenGenerator(ScheduleDbContext context) {
            _context = context;
        }

        public string GenerateRefreshToken(string username) {
            //var randomNumber = new byte[32];
            //using (var randomNumberGenerator = RandomNumberGenerator.Create()) {
            //    randomNumberGenerator.GetBytes(randomNumber);
            //    string refreshToken = Convert.ToBase64String(randomNumber);

            //    var user = _context.RefreshTokens.FirstOrDefault(x => x.Username == username);
            //    if (user != null) {
            //        user
            //    }

            //    return refreshToken;
            //}
            return "";
        }
    }
}
