using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.ScheduleDtos {
    public class SaveEmployeeDaySchedule {
        public int EmployeeId { get; set; }
        public int Day { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
    }
    public class SaveWeeklyScheduleDto {
        public int Week { get; set; }
        public int Year { get; set; }
        public List<SaveEmployeeDaySchedule>? Schedules { get; set; }

    }
}
