using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleApi.Data;
using ScheduleApi.Dtos.ScheduleDtos;
using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using static ScheduleApi.Utils.Utils;

namespace ScheduleApi.Services.ScheduleService {
    public class ScheduleService : IScheduleService {
        private ScheduleDbContext _context;
        private IHttpContextAccessor _contextAccessor;
        private IMapper _mapper;

        public ScheduleService(ScheduleDbContext context, IHttpContextAccessor contextAccessor, IMapper mapper) {
            _context = context;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public async Task<GetScheduleDto> AddSchedule(AddScheduleDto scheduleToAdd) {
            await _context.CheckEmployeeUserValid(scheduleToAdd.EmployeeId, _contextAccessor);

            Schedule schedule = _mapper.Map<Schedule>(scheduleToAdd);

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return _mapper.Map<GetScheduleDto>(schedule);
        }

        public async Task DeleteSchedule(EmployeeDayScheduleDto request) {
            await _context.CheckEmployeeUserValid(request.EmployeeId, _contextAccessor);
            Schedule? toDelete = await _context.Schedules.FirstOrDefaultAsync(s => 
                s.Week == request.Week &&
                s.Year == request.Year &&
                s.Day == request.Day &&
                s.EmployeeId == request.EmployeeId);

            if (toDelete == null)
                throw new KeyNotFoundException(ScheduleNotFoundMessage(request));


            _context.Schedules.Remove(toDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetScheduleDto>?> GetAllSchedules() {
            string userId = GetUserId(_contextAccessor);
            List<Schedule>? schedules = await _context.Schedules
                .Include(r => r.Employee)
                .Where(r => r.Employee.UserId == userId)
                .ToListAsync();

            return schedules?.Select(s => _mapper.Map<GetScheduleDto>(s)).ToList();
        }

        public async Task<GetScheduleDto> GetScheduleById(int id) {
            Schedule? schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ID == id);

            if (schedule == null)
                throw new KeyNotFoundException(IdNotFoundMessage("schedule", id));

            await _context.CheckUserValid("schedule", id, schedule.EmployeeId, _contextAccessor);

            return _mapper.Map<GetScheduleDto>(schedule);
        }

        public async Task<GetScheduleDto> GetScheduleByDayEmployee(EmployeeDayScheduleDto request) {
            await _context.CheckEmployeeUserValid(request.EmployeeId, _contextAccessor);

            Schedule? schedule = await _context.Schedules.FirstOrDefaultAsync(s =>
                s.Week == request.Week &&
                s.Year == request.Year &&
                s.EmployeeId == request.EmployeeId &&
                s.Day == request.Day);

            if (schedule == null)
                throw new KeyNotFoundException(ScheduleNotFoundMessage(request));

            return _mapper.Map<GetScheduleDto>(schedule);
        }

        public async Task<IEnumerable<GetScheduleDto>> GetSchedulesByEmployeeId(int id) {
            await _context.CheckEmployeeUserValid(id, _contextAccessor);

            List<Schedule> schedules = await _context.Schedules
                .Where(s => s.EmployeeId == id)
                .ToListAsync();

            return schedules.Select(s => _mapper.Map<GetScheduleDto>(s));
        }

        public async Task<IEnumerable<GetScheduleDto>> GetSchedulesByWeek(WeeklyScheduleDto request) {
            string userId = GetUserId(_contextAccessor);

            List<Schedule> schedules = await _context.Schedules
                .Include(s => s.Employee)
                .Where(s => 
                    s.Week == request.week && 
                    s.Year == request.year && 
                    s.Employee.UserId == userId)
                .ToListAsync();

            return schedules.Select(s => _mapper.Map<GetScheduleDto>(s));
        }

        public async Task<IEnumerable<GetScheduleDto>> GetSchedulesByWeekEmployee(
            EmployeeWeeklyScheduleDto request) {
            string userId = GetUserId(_contextAccessor);

            List<Schedule> schedules = await _context.Schedules
                .Include(s => s.Employee)
                .Where(s =>
                    s.EmployeeId == request.employeeId &&
                    s.Week == request.week &&
                    s.Year == request.year &&
                    s.Employee.UserId == userId)
                .ToListAsync();

            return schedules.Select(s => _mapper.Map<GetScheduleDto>(s));

        }

        public async Task<IEnumerable<GetScheduleDto>> SaveWeeklySchedule(SaveWeeklyScheduleDto request) {
            if (request.Schedules == null || request.Schedules.Count < 1)
                throw new AppException("No schedules provided to save");

            string userId = GetUserId(_contextAccessor);

            foreach (SaveEmployeeDaySchedule toSave in request.Schedules) { 
                if (await _context.Schedules
                    .Include(s => s.Employee)
                    .AnyAsync(s => 
                        s.EmployeeId == toSave.EmployeeId && 
                        s.Employee.UserId != userId)) {
                    throw new KeyNotFoundException(IdNotFoundMessage("employee", toSave.EmployeeId));
                }
            }
            
            List<GetScheduleDto> updated = new List<GetScheduleDto>();

            foreach(SaveEmployeeDaySchedule toSave in request.Schedules) {
                Schedule? schedule = await _context.Schedules
                    .Include(s => s.Employee)
                    .FirstOrDefaultAsync(s => 
                        s.Year == request.Year &&
                        s.Week == request.Week &&
                        s.Day == toSave.Day &&
                        s.EmployeeId == toSave.EmployeeId);

                if (schedule == null) {
                    Schedule toAdd = _context.Schedules.Add(new Schedule {
                        Year = request.Year,
                        Week = request.Week,
                        Day = toSave.Day,
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
            await _context.CheckEmployeeUserValid(updatedSchedule.EmployeeId, _contextAccessor);

            Schedule? schedule = await _context.Schedules
                .FirstOrDefaultAsync(s => 
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

            if (schedule.EmployeeId != updatedSchedule.EmployeeId)
                throw new AppException(string.Format(
                    "employeeId = {0} does not match that of stored value = {1}", 
                    updatedSchedule.EmployeeId, schedule.EmployeeId));

            schedule.Start = updatedSchedule.Start;
            schedule.End = updatedSchedule.End;
            schedule.Week = updatedSchedule.Week;
            schedule.Day = updatedSchedule.Day;
            schedule.Year = updatedSchedule.Year;

            await _context.SaveChangesAsync();

            return _mapper.Map<GetScheduleDto>(schedule);
        }

        private string ScheduleNotFoundMessage(Object data) {
            return string.Format("schedule not found matching {0}", data.ToString());
        }
    }
}
