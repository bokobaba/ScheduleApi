
namespace ScheduleApi.Dtos.RequestDtos {
    public class AddRequestDto {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int EmployeeId { get; set; }
    }
}
