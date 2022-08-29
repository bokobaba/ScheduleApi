using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.UserDtos {
    public class RefreshTokenRequest {
        [Required]
        public string ExpiredToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
