using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.AvailabilityDtos {
    public class UpdateEmployeeAvailabilityDto {
        [Range(0, int.MaxValue)]
        public int EmployeeId { get; set; }
        [Required]
        public List<UpdateAvailabilityDto> EmployeeAvailability { get; set; }
    }
}
