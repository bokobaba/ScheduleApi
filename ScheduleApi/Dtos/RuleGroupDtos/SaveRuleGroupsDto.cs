using System.ComponentModel.DataAnnotations;

namespace ScheduleApi.Dtos.RuleGroupDtos {
    public class SaveRuleGroupsDto {
        [Required]
        public List<SaveRuleGroupDto> Rules { get; set; }
    }
}
