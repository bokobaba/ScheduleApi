using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.RequestDtos {
    public class UpdateRequestDto {
        [Required]
        public int ID { get; set; }
        [Required]
        public DateTime? Start { get; set; }
        [Required]
        public DateTime? End { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int EmployeeId { get; set; }
    }
}
