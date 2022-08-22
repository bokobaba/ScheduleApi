﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleApi.Models {
    public class Schedule {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Week { get; set; }
        [Required]
        public int Day { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }

        public Schedule(int employeeId, int day, int week, int year) {
            EmployeeId = employeeId;
            Day = day;
            Year = year;
            Week = week;
            Start = DateTime.Now;
            End = DateTime.Now;
        }
    }
}