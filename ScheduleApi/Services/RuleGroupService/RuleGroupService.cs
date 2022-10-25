using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleApi.Data;
using ScheduleApi.Dtos.RuleGroupDtos;
using ScheduleApi.Models;
using static ScheduleApi.Utils.Utils;

namespace ScheduleApi.Services.RuleGroupService {
    public class RuleGroupService : IRuleGroupService {
        private ScheduleDbContext _context;
        private IHttpContextAccessor _contextAccessor;
        private IMapper _mapper;

        public RuleGroupService(ScheduleDbContext context, IHttpContextAccessor contextAccessor,
            IMapper mapper) {
            _context = context;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetRuleGroupDto>> GetRuleGroups() {
            string userId = GetUserId(_contextAccessor);
            List<RuleGroup> ruleGroups = await _context.RuleGroups
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return ruleGroups.Select(r => _mapper.Map<GetRuleGroupDto>(r)).ToList();
        }

        public async Task<IEnumerable<GetRuleGroupDto>> SaveRuleGroups(SaveRuleGroupsDto ruleGroups) {
            string userId = GetUserId(_contextAccessor);
            List<RuleGroup> toSave = ruleGroups.Rules.Select(r => {
                RuleGroup rule = _mapper.Map<RuleGroup>(r);
                rule.UserId = userId;
                return rule;
            }).ToList();

            List<RuleGroup> toRemove = await _context.RuleGroups
                .Where(r => r.UserId == userId)
                .ToListAsync();

            _context.RuleGroups.RemoveRange(toRemove);
            _context.RuleGroups.AddRange(toSave);

            await _context.SaveChangesAsync();

            return toSave.Select(r => _mapper.Map<GetRuleGroupDto>(r)).ToList();
        }
    }
}
