namespace ScheduleApi.Dtos.ScheduleDtos {
    public class GetScheduleDto {
        public int Year { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }

        public int EmployeeId { get; set; }
    }
}
