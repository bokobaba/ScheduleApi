
using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.RequestDtos {
    public class AddRequestDto {
        [Required]
        public DateTime? Start { get; set; }
        [Required]
        public DateTime? End { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}
