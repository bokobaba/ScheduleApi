using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.RequestDtos {
    public class GetRequestDto {
        public int ID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public int EmployeeId { get; set; }
    }
}
