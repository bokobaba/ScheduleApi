using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Models;

namespace ScheduleApi.Dtos.EmployeeDtos {
    public class AddEmployeeDto {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public List<AddEmployeeRequestDto>? Requests { get; set; }
    }
}
