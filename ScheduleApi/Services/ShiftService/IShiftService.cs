using ScheduleApi.Dtos.ShiftDtos;

namespace ScheduleApi.Services.ShiftService {
    public interface IShiftService {
        public Task<IEnumerable<GetShiftDto>> GetAllShifts();
        public Task<GetShiftDto> GetById(int id);
        public Task<GetShiftDto> CreateShifts(AddShiftDto request);
        public Task DeleteShift(int id);
        public Task<GetShiftDto> UpdateShift(UpdateShiftDto request);
    }
}
