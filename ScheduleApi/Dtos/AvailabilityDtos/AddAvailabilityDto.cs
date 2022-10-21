using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.AvailabilityDtos {
    public class AddAvailabilityDto {
        [Required]
        public int EmployeeId { get; set; }
    }
}
