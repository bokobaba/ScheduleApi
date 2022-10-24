using ScheduleApi.Dtos.AvailabilityDtos;
using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Dtos.ScheduleDtos;

namespace ScheduleApi.Dtos.EmployeeDtos {
    public class GetEmployeeInfoDto {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public List<GetRequestDto> Requests { get; set; }
        public List<GetAvailabilityDto> Availability { get; set; }
        public List<GetScheduleDto> Schedule { get; set; }
    }
}
