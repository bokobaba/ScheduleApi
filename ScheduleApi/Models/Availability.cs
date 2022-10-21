using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleApi.Models {
    public class Availability {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Day { get; set; }
        public bool Enabled { get; set; }
        public bool AllDay { get; set; }
        public string Start { get; set; }
        public string End { get; set; }

        public string UserId { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
