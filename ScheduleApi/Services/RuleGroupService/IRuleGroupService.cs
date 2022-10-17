using ScheduleApi.Dtos.RuleGroupDtos;

namespace ScheduleApi.Services.RuleGroupService {
    public interface IRuleGroupService {
        Task<IEnumerable<GetRuleGroupDto>> GetRuleGroups();
        Task<IEnumerable<GetRuleGroupDto>> SaveRuleGroups(SaveRuleGroupsDto ruleGroups);
    }
}
