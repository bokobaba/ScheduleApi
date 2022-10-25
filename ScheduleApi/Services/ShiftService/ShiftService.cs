using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleApi.Data;
using ScheduleApi.Dtos.ShiftDtos;
using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using static ScheduleApi.Utils.Utils;

namespace ScheduleApi.Services.ShiftService {
    public class ShiftService : IShiftService {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ScheduleDbContext _context;
        private readonly IMapper _mapper;

        public ShiftService(IHttpContextAccessor contextAccessor, ScheduleDbContext context, IMapper mapper) {
            _contextAccessor = contextAccessor;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetShiftDto>> GetAllShifts() {
            string userId = GetUserId(_contextAccessor);

            List<Shift> shifts = await _context.Shifts
                .AsNoTracking()
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return shifts.Select(s => _mapper.Map<GetShiftDto>(s));
        }

        public async Task<GetShiftDto> GetById(int id) {
            string userId = GetUserId(_contextAccessor);

            Shift? shift = await _context.Shifts
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == id);

            if (shift == null || shift.UserId != userId) {
                throw new KeyNotFoundException(IdNotFoundMessage("shift", id));
            }

            return _mapper.Map<GetShiftDto>(shift);
        }

        public async Task<GetShiftDto> CreateShifts(AddShiftDto request) {
            string userId = GetUserId(_contextAccessor);

            Shift shift = _mapper.Map<Shift>(request);
            shift.UserId = userId;

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            return _mapper.Map<GetShiftDto>(shift);
        }

        public async Task DeleteShift(int id) {
            string userId = GetUserId(_contextAccessor);

            Shift? shift = await _context.Shifts.FirstOrDefaultAsync(s => s.ID == id);

            if (shift == null || shift.UserId != userId)
                throw new KeyNotFoundException(IdNotFoundMessage("shift", id));

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();
        }

        public async Task<GetShiftDto> UpdateShift(UpdateShiftDto request) {
            string userId = GetUserId(_contextAccessor);

            Shift? shift = await _context.Shifts.FirstOrDefaultAsync(s => s.ID == request.Id);

            if (shift == null || shift.UserId != userId) 
                throw new KeyNotFoundException(IdNotFoundMessage("shift", request.Id));

            shift.Name = request.Name;
            shift.Start = request.Start;
            shift.End = request.End;

            await _context.SaveChangesAsync();

            return _mapper.Map<GetShiftDto>(shift);

        }
    }
}
