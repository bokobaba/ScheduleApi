namespace ScheduleApi.Dtos.RuleGroupDtos {
    public class GetRuleGroupDto {
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool Status { get; set; }
        public string Rules { get; set; }
    }
}
