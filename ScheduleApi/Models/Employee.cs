﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleApi.Models {
    [Index(nameof(EmployeeId), IsUnique = true)]
    public class Employee {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public string Name { get; set; }
        
        public string UserId { get; set; }

        public List<Request>? Requests { get; set; }
        public List<Schedule>? Schedules { get; set; }
    }
}
