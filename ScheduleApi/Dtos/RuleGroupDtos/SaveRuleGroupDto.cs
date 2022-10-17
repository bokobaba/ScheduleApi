using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.RuleGroupDtos {
    public class SaveRuleGroupDto {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Priority { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public string Rules { get; set; }
    }
}
