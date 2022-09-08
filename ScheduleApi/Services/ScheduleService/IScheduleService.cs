using ScheduleApi.Dtos.ScheduleDtos;

namespace ScheduleApi.Services.ScheduleService {
    public interface IScheduleService {
        public Task<GetScheduleDto> AddSchedule(AddScheduleDto scheduleToAdd);
        public Task<IEnumerable<GetScheduleDto>?> GetAllSchedules();
        public Task<GetScheduleDto> GetScheduleById(int id);
        public Task<IEnumerable<GetScheduleDto>> GetSchedulesByEmployeeId(int id);
        public Task<IEnumerable<GetScheduleDto>> GetSchedulesByWeek(WeeklyScheduleDto request);
        public Task<IEnumerable<GetScheduleDto>> GetSchedulesByWeekEmployee(EmployeeWeeklyScheduleDto request);
        public Task<GetScheduleDto> GetScheduleByDayEmployee(EmployeeDayScheduleDto request);
        public Task<GetScheduleDto> UpdateSchedule(UpdateScheduleDto updatedSchedule);
        public Task<IEnumerable<GetScheduleDto>> SaveWeeklySchedule(SaveWeeklyScheduleDto request);
        public Task DeleteSchedule(EmployeeDayScheduleDto request);
    }
}
