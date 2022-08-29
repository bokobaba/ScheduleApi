namespace ScheduleApi {
    public interface IRefreshTokenGenerator {
        string GenerateRefreshToken(string username);
    }
}
