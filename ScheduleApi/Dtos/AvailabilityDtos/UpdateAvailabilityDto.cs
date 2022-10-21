using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.AvailabilityDtos {
    public class UpdateAvailabilityDto {
        [Range(0, 7, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Day { get; set; }
        [Required]
        public bool Enabled { get; set; }
        [Required]
        public bool AllDay { get; set; }
        [Required]
        public string Start { get; set; }
        [Required]
        public string End { get; set; }
    }
}
