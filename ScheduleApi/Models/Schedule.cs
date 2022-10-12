using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleApi.Models {
    public class Schedule {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Week { get; set; }
        [Required]
        public int Day { get; set; }
        public string? Start { get; set; }
        public string? End { get; set; }

        public string UserId { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public Schedule() {

        }

        public Schedule(int employeeId, int day, int week, int year) {
            EmployeeId = employeeId;
            Day = day;
            Year = year;
            Week = week;
            Start = "";
            End = "";
        }
    }
}
