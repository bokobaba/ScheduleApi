using ScheduleApi.Dtos.AvailabilityDtos;

namespace ScheduleApi.Services.AvailabilityService {
    public interface IAvailabilityService {
        public Task<IEnumerable<GetAvailabilityDto>> GetAllAvailabilities();
        public Task<IEnumerable<GetAvailabilityDto>> GetAvailabilityForEmployee(int id);
        public Task<IEnumerable<GetAvailabilityDto>> CreateAvailability(int id);
        public Task<IEnumerable<GetAvailabilityDto>> UpdateAvailability(UpdateEmployeeAvailabilityDto request);
        public Task DeleteAvailability(int id);
    }
}
