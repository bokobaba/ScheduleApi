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

            List<Availability> toUpdate = await _context.Availability
                .Where(a => a.UserId == userId && a.EmployeeId == request.EmployeeId)
                .ToListAsync();

            toUpdate.ForEach(a => {
                UpdateAvailabilityDto? update = request.EmployeeAvailability
                .Find(r => r.Day == a.Day);

                if (update == null)
                    throw new AppException(string.Format("missing day in request: {0}", a.Day));

                a.Enabled = update.Enabled;
                a.AllDay = update.AllDay;
                a.Start = update.Start;
                a.End = update.End;
            });

            await _context.SaveChangesAsync();

            return toUpdate.Select(u => _mapper.Map<GetAvailabilityDto>(u));
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
