using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Models {
    public class RuleGroup {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool Status { get; set; }
        public string Rules { get; set; }
        public string UserId { get; set; }
    }
}
