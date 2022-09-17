using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.ScheduleDtos {
    public class WeeklyScheduleDto {
        [Range(1, int.MaxValue)]
        public int week { get; set; }
        [Range(1, int.MaxValue)]
        public int year { get; set; }
    }
}
