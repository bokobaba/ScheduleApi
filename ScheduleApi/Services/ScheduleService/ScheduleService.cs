using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleApi.Data;
using ScheduleApi.Dtos.ScheduleDtos;
using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using static ScheduleApi.Utils.Utils;
using static ScheduleApi.Utils.ScheduleUtils;
using ScheduleApi.Utils;
using AutoMapper.Execution;

namespace ScheduleApi.Services.ScheduleService {
    public class ScheduleService : IScheduleService {
        private readonly ScheduleDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ScheduleService(ScheduleDbContext context, IHttpContextAccessor contextAccessor, 
            IMapper mapper, ILogger<ScheduleService> logger) {
            _context = context;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetScheduleDto> AddSchedule(AddScheduleDto scheduleToAdd) {
            await _context.CheckEmployeeUserValid(scheduleToAdd.EmployeeId, _contextAccessor);

            Schedule schedule = _mapper.Map<Schedule>(scheduleToAdd);
            schedule.UserId = GetUserId(_contextAccessor);

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return _mapper.Map<GetScheduleDto>(schedule);
        }

        public async Task DeleteSchedule(EmployeeDayScheduleDto request) {
            string userId = GetUserId(_contextAccessor);
            Schedule? toDelete = await _context.Schedules.FirstOrDefaultAsync(s =>
                s.Week == request.Week &&
                s.Year == request.Year &&
                s.Day == request.Day &&
                s.UserId == userId &&
                s.EmployeeId == request.EmployeeId);

            if (toDelete == null)
                throw new KeyNotFoundException(ScheduleNotFoundMessage(request));


            _context.Schedules.Remove(toDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetScheduleDto>?> GetAllSchedules() {
            string userId = GetUserId(_contextAccessor);
            List<Schedule>? schedules = await _context.Schedules
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return schedules?.Select(s => _mapper.Map<GetScheduleDto>(s)).ToList();
        }

        public async Task<GetScheduleDto> GetScheduleById(int id) {
            string userId = GetUserId(_contextAccessor);
            Schedule? schedule = await _context.Schedules
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == id && s.UserId == userId);

            if (schedule == null || schedule.UserId != GetUserId(_contextAccessor))
                throw new KeyNotFoundException(IdNotFoundMessage("schedule", id));

            return _mapper.Map<GetScheduleDto>(schedule);
        }

        public async Task<GetScheduleDto> GetScheduleByDayEmployee(EmployeeDayScheduleDto request) {
            string userId = GetUserId(_contextAccessor);

            Schedule? schedule = await _context.Schedules
                .AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    s.Week == request.Week &&
                    s.Year == request.Year &&
                    s.UserId == userId &&
                    s.EmployeeId == request.EmployeeId &&
                    s.Day == request.Day);

            if (schedule == null)
                throw new KeyNotFoundException(ScheduleNotFoundMessage(request));

            return _mapper.Map<GetScheduleDto>(schedule);
        }

        public async Task<IEnumerable<GetScheduleDto>> GetSchedulesByEmployeeId(int id) {
            string userId = GetUserId(_contextAccessor);
            List<Schedule> schedules = await _context.Schedules
                .AsNoTracking()
                .Where(s => s.UserId == userId && s.EmployeeId == id)
                .ToListAsync();

            return schedules.Select(s => _mapper.Map<GetScheduleDto>(s));
        }

        public async Task<IEnumerable<GetScheduleDto>> GetSchedulesByWeek(WeeklyScheduleDto request) {
            string userId = GetUserId(_contextAccessor);

            List<Schedule> schedules = await _context.Schedules
                .AsNoTracking()
                .Where(s =>
                    s.Week == request.week &&
                    s.Year == request.year &&
                    s.UserId == userId)
                .ToListAsync();

            return schedules.Select(s => _mapper.Map<GetScheduleDto>(s));
        }

        public async Task<IEnumerable<GetScheduleDto>> GetSchedulesByWeekEmployee(
            EmployeeWeeklyScheduleDto request) {
            string userId = GetUserId(_contextAccessor);

            List<Schedule> schedules = await _context.Schedules
                .AsNoTracking()
                .Where(s =>
                    s.Week == request.week &&
                    s.Year == request.year &&
                    s.UserId == userId &&
                    s.EmployeeId == request.employeeId)
                .ToListAsync();

            return schedules.Select(s => _mapper.Map<GetScheduleDto>(s));

        }

        public async Task<IEnumerable<GetScheduleDto>> SaveWeeklySchedule(SaveWeeklyScheduleDto request) {
            List<GetScheduleDto> updated = new();

            if (request.Schedules == null || request.Schedules.Count < 1)
                return updated;

            string userId = GetUserId(_contextAccessor);

            foreach (SaveEmployeeDaySchedule toSave in request.Schedules) {
                Schedule? schedule = await _context.Schedules
                    .FirstOrDefaultAsync(s =>
                        s.UserId == userId &&
                        s.Year == request.Year &&
                        s.Week == request.Week &&
                        s.Day == toSave.Day &&
                        s.EmployeeId == toSave.EmployeeId);

                if (schedule == null) {
                    Schedule toAdd = _context.Schedules.Add(new Schedule {
                        Year = request.Year,
                        Week = request.Week,
                        Day = toSave.Day,
                        UserId = userId,
                        EmployeeId = toSave.EmployeeId,
                        Start = toSave.Start,
                        End = toSave.End,
                    }).Entity;

                    updated.Add(_mapper.Map<GetScheduleDto>(toAdd));
                } else {
                    schedule.Start = toSave.Start;
                    schedule.End = toSave.End;

                    updated.Add(_mapper.Map<GetScheduleDto>(schedule));
                }

                await _context.SaveChangesAsync();
            }

            return updated;
        }

        public async Task<GetScheduleDto> UpdateSchedule(UpdateScheduleDto updatedSchedule) {
            string userId = GetUserId(_contextAccessor);
            Schedule? schedule = await _context.Schedules
                .FirstOrDefaultAsync(s =>
                    s.UserId == userId &&
                    s.Week == updatedSchedule.Week &&
                    s.Year == updatedSchedule.Year &&
                    s.EmployeeId == updatedSchedule.EmployeeId &&
                    s.Day == updatedSchedule.Day);

            if (schedule == null)
                throw new KeyNotFoundException(ScheduleNotFoundMessage(
                    new {
                        updatedSchedule.Day,
                        updatedSchedule.Week,
                        updatedSchedule.Year,
                        updatedSchedule.EmployeeId
                    }));

            schedule.Start = updatedSchedule.Start;
            schedule.End = updatedSchedule.End;
            schedule.Week = updatedSchedule.Week;
            schedule.Day = updatedSchedule.Day;
            schedule.Year = updatedSchedule.Year;

            await _context.SaveChangesAsync();

            return _mapper.Map<GetScheduleDto>(schedule);
        }

        public async Task<IEnumerable<GetScheduleDto>> GenerateScheduleFromRules(GenerateScheduleDto request) {
            string userId = GetUserId(_contextAccessor);

            List<RuleGroup> rules = await _context.RuleGroups
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .ToListAsync();
            List<Shift> shifts = await _context.Shifts
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .ToListAsync();
            List<Employee> employees = await _context.Employees
                .AsNoTrackingWithIdentityResolution()
                .AsSingleQuery()
                .Where(e => e.UserId == userId)
                .Include(e => e.Requests).Include(e => e.Availability)
                .ToListAsync();

            List<Schedule> schedules = GenerateSchedules(rules, shifts, employees);
            schedules.ForEach(s => {
                s.UserId = userId;
                s.Year = request.Year;
                s.Week = request.Week;
            });

            return schedules.Select(s => _mapper.Map<GetScheduleDto>(s));
        }

        private static string ScheduleNotFoundMessage(Object data) {
            return string.Format("schedule not found matching {0}", data.ToString());
        }

        private List<Schedule> GenerateSchedules(List<RuleGroup> ruleGroups, 
            List<Shift> shifts, List<Employee> employees) {
            List<Rule> rules = ruleGroups.Select(r => ParseRule(r, employees, shifts)).ToList();
            rules.Sort((a, b) => a.Priority - b.Priority);
            //rules.ForEach(r => Console.WriteLine(r));

            ScheduleRequirements reqs = new ScheduleRequirements(rules);
            List<Schedule> schedules = reqs.GenerateSchedule(employees);

            return schedules;
        }

        // rules separated by ; operator separated by , type separated by :
        private static Rule ParseRule(RuleGroup ruleGroup, List<Employee> employees, List<Shift> shifts) {
            Rule result = new() { Priority = ruleGroup.Priority, Status = ruleGroup.Status };

            string[] rules = ruleGroup.Rules.Split(";");
            if (rules.Length == 0) return result;

            foreach (string rule in rules) {
                if (rule == null || rule.Equals(""))
                    continue;

                string[] properties = rule.Split(":");

                if (properties.Length < 2)
                    throw new AppException("incorrectly formatted rule");

                string[] description = properties[1].Split(",");
                if (description.Length < 1)
                    throw new AppException("incorrectly formatted rule");

                string type = properties[0].ToLower();

                switch (type) {
                    case "hours":
                        result.Hours = ParseHours(description);
                        break;
                    case "employee":
                        result.Employees.AddRange(ParseEmployee(description, employees));
                        break;
                    case "day":
                        result.Days.Add(ParseDay(description));
                        break;
                    case "shift":
                        Condition? c = ParseShift(description, shifts);
                        if (c != null)
                            result.Shift = c;
                        break;
                    default:
                        throw new AppException("invalid identifier: " + type);
                }
            }

            return result;
        }

        private static Condition ParseHours(string[] description) {
            if (description.Length != 2 || !int.TryParse(description[1], out _))
                throw new AppException("incorrectly formatted rule");

            return new Condition() {
                Name = "hours",
                Type = ConditionTypes.HOURS,
                Operator = description[0].ToLower(),
                Value = int.Parse(description[1]),
            };
        }

        private static Condition? ParseShift(string[] description, List<Shift> shifts) {
            if (description.Length != 1 || !int.TryParse(description[0], out int id))
                throw new AppException("incorrectly formatted rule");
            Shift? s = shifts.Find(s => s.ID == id);
            if (s == null)
                return null;

            return new Condition() {
                Name = "shift",
                Type = ConditionTypes.SHIFT,
                Value = id,
                Shift = new TimeRange() {
                    Start = s.Start,
                    End = s.End
                }
            };
        }

        private static List<Condition> ParseEmployee(string[] description, List<Employee> employees) {
            bool opPresent = description.Length > 1;
            string name = opPresent ? description[1].ToLower() : description[0].ToLower();
            string? op = opPresent ? description[0].ToLower() : null;

            if (name.Equals("all")) {
                List<Condition> conditions = new();
                employees.ForEach(e => {
                    conditions.Add(new Condition() {
                        Name = name,
                        Type = ConditionTypes.EMPLOYEE,
                        Operator = op,
                        Value = e.EmployeeId
                    });
                });
                return conditions;
            }
            string str = opPresent ? description[1] : description[0];
            if (!int.TryParse(str, out int value))
                throw new AppException("incorrectly formatted rule");

            return new List<Condition>() {
                new Condition() {
                    Name = name,
                    Type = ConditionTypes.EMPLOYEE,
                    Operator = op,
                    Value = value
            }};
        }

        private static Condition ParseDay(string[] description) {
            bool opPresent = description.Length > 1;
            string name = opPresent ? description[1] : description[0];
            string? op = opPresent ? description[0].ToLower() : null;

            return new Condition() {
                Name = name.ToLower(),
                Type = ConditionTypes.DAY,
                Operator = op
            };
        }
    }
}
