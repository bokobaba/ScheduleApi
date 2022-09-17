using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.RequestDtos {
    public class AddEmployeeRequestDto {
        [Required]
        public DateTime? Start { get; set; }
        [Required]
        public DateTime? End { get; set; }
    }
}
