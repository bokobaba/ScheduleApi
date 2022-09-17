using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.ScheduleDtos {
    public class SaveEmployeeDaySchedule {
        public int EmployeeId { get; set; }
        [Range(1, int.MaxValue)]
        public int Day { get; set; }
        [Required]
        public DateTime? Start { get; set; }
        [Required]
        public DateTime? End { get; set; }
    }
    public class SaveWeeklyScheduleDto {
        [Range(1, int.MaxValue)]
        public int Week { get; set; }
        [Range(1, int.MaxValue)]
        public int Year { get; set; }
        [Required]
        public List<SaveEmployeeDaySchedule>? Schedules { get; set; }

    }
}
