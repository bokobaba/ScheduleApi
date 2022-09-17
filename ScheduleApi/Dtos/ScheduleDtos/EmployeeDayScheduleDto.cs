﻿using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.ScheduleDtos {
    public class EmployeeDayScheduleDto {
        [Range(1, int.MaxValue)]
        public int Year { get; set; }
        [Range(1, int.MaxValue)]
        public int Week { get; set; }
        [Range(1, int.MaxValue)]
        public int Day { get; set; }
        public int EmployeeId { get; set; }
    }
}
