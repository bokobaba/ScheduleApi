using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleApi.Data;
using ScheduleApi.Dtos.AvailabilityDtos;
using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using static ScheduleApi.Utils.Utils;

namespace ScheduleApi.Services.AvailabilityService {
    public class AvailabilityService : IAvailabilityService {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ScheduleDbContext _context;
        private readonly IMapper _mapper;

        public AvailabilityService(IHttpContextAccessor contextAccessor, ScheduleDbContext context, IMapper mapper) {
            _contextAccessor = contextAccessor;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GetAvailabilityDto>> GetAllAvailabilities() {
            string userId = GetUserId(_contextAccessor);

            List<Availability> availabilities = await _context.Availability
                .Where(a => a.UserId == userId)
                .ToListAsync();

            return availabilities.Select(a => _mapper.Map<GetAvailabilityDto>(a));
        }

        public async Task<IEnumerable<GetAvailabilityDto>> GetAvailabilityForEmployee(int id) {
            string userId = GetUserId(_contextAccessor);

            List<Availability> availabilities = await _context.Availability
                .Where(a => a.UserId == userId && a.EmployeeId == id)
                .ToListAsync();

            return availabilities.Select(a => _mapper.Map<GetAvailabilityDto>(a));
        }

        public async Task<IEnumerable<GetAvailabilityDto>> CreateAvailability(int id) {
            await _context.CheckEmployeeUserValid(id, _contextAccessor);
            string userId = GetUserId(_contextAccessor);

            // create Availability for each day of week
            List<Availability> toAdd = new List<Availability>();
            for (int i = 0; i < 7; ++i) {
                toAdd.Add(new Availability {
                    UserId = userId,
                    EmployeeId = id,
                    Day = i,
                    AllDay = true,
                    Enabled = true,
                    Start = "",
                    End = ""
                });
            }

            _context.Availability.AddRange(toAdd);
            await _context.SaveChangesAsync();

            return toAdd.Select(a => _mapper.Map<GetAvailabilityDto>(a));
        }

        public async Task<IEnumerable<GetAvailabilityDto>> UpdateAvailability(
            UpdateEmployeeAvailabilityDto request) {
            string userId = GetUserId(_contextAccessor);

            List<Availability> updated = new List<Availability>();
            foreach(UpdateAvailabilityDto update in request.EmployeeAvailability) {
                Availability? entry = await _context.Availability
                .FirstOrDefaultAsync(a =>
                    a.UserId == userId &&
                    a.EmployeeId == request.EmployeeId &&
                    a.Day == update.Day);
                if (entry == null)
                    throw new KeyNotFoundException(string.Format(
                        "Availaibility not found for employee {0} on day {1}",
                        request.EmployeeId, update.Day));

                entry.Enabled = update.Enabled;
                entry.AllDay = update.AllDay;
                entry.Start = update.Start;
                entry.End = update.End;

                updated.Add(entry);
            };

            await _context.SaveChangesAsync();

            return updated.Select(u => _mapper.Map<GetAvailabilityDto>(u));
        }

        public async Task DeleteAvailability(int id) {
            string userId = GetUserId(_contextAccessor);

            List<Availability> toDelete = await _context.Availability
                .Where(a => a.UserId == userId && a.EmployeeId == id)
                .ToListAsync();

            if (toDelete.Count == 0)
                throw new KeyNotFoundException("Availability not found for employee " + id);

            _context.Availability.RemoveRange(toDelete);
            await _context.SaveChangesAsync();
        }
    }
}
