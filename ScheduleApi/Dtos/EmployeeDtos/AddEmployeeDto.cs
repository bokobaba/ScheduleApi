using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Models;
using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.EmployeeDtos {
    public class AddEmployeeDto {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public string Name { get; set; }
        public List<AddEmployeeRequestDto>? Requests { get; set; }
    }
}
