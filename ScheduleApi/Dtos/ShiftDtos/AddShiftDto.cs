using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.ShiftDtos {
    public class AddShiftDto {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Start { get; set; }
        [Required]
        public string End { get; set; }
    }
}
