using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.UserDtos {
    public class UserLoginDto {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
