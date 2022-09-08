namespace ScheduleApi.Dtos.ScheduleDtos {
    public class GetScheduleDto {
        public int Year { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public int EmployeeId { get; set; }
    }
}
