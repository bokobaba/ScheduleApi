using ScheduleApi.Dtos.RequestDtos;

namespace ScheduleApi.Dtos.EmployeeDtos {
    public class UpdateEmployeeDto {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public List<AddEmployeeRequestDto>? Requests { get; set; }
    }
}
