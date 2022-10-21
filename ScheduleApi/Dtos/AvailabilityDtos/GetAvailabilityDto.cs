namespace ScheduleApi.Dtos.AvailabilityDtos {
    public class GetAvailabilityDto {
        public int Day { get; set; }
        public bool Enabled { get; set; }
        public bool AllDay { get; set; }
        public string Start { get; set; }
        public string End { get; set; }

        public int EmployeeId { get; set; }
    }
}
