using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.ScheduleDtos {
    public class GenerateScheduleDto {
        [Range(1, int.MaxValue)]
        public int Week { get; set; }
        public int Year { get; set; }
    }
}
