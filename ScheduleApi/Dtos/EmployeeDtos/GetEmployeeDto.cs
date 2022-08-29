using ScheduleApi.Dtos.RequestDtos;

namespace ScheduleApi.Dtos.EmployeeDtos {
    public class GetEmployeeDto {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public List<GetRequestDto>? Requests { get; set; }
    }
}
