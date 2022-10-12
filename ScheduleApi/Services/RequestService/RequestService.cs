using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleApi.Data;
using ScheduleApi.Dtos.RequestDtos;
using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using static ScheduleApi.Utils.Utils;

namespace ScheduleApi.Services.RequestService {
    public class RequestService : IRequestService {
        private readonly ScheduleDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public RequestService(ScheduleDbContext context, IMapper mapper, IHttpContextAccessor contextAccessor) {
            _context = context;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public async Task<GetRequestDto> AddRequest(AddRequestDto newRequest) {
            await _context.CheckEmployeeUserValid(newRequest.EmployeeId, _contextAccessor);

            Request request = _mapper.Map<Request>(newRequest);
            request.UserId = GetUserId(_contextAccessor);

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return _mapper.Map<GetRequestDto>(request);
        }

        public async Task DeleteRequest(int id) {
            Request? toDelete = await _context.Requests.FirstOrDefaultAsync(r => r.ID == id);

            if (toDelete == null || toDelete.UserId != GetUserId(_contextAccessor))
                throw new KeyNotFoundException(IdNotFoundMessage("request", id));

            //await _context.CheckUserValid("request", id, toDelete.EmployeeId, _contextAccessor);

            _context.Requests.Remove(toDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetRequestDto>?> GetAllRequests() {
            string userId = GetUserId(_contextAccessor);
            IEnumerable<Request>? requests = await _context.Requests
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return requests?.Select(r => _mapper.Map<GetRequestDto>(r)).ToList();
        }

        public async Task<GetRequestDto> GetReqeustById(int id) {
            Request? request = await _context.Requests.FirstOrDefaultAsync(r => r.ID == id);
            if (request == null || request.UserId != GetUserId(_contextAccessor))
                throw new KeyNotFoundException(IdNotFoundMessage("request", id));

            return _mapper.Map<GetRequestDto>(request);
        }

        public async Task<IEnumerable<GetRequestDto>?> GetReqeustsByEmployeeId(int employeeId) {
            string userId = GetUserId(_contextAccessor);
            IEnumerable<Request>? requests = await _context.Requests
                .Where(r => r.UserId == userId && r.EmployeeId == employeeId)
                .ToListAsync();

            return requests?.Select(r => _mapper.Map<GetRequestDto>(r)).ToList();
        }

        public async Task<GetRequestDto> UpdateRequest(UpdateRequestDto updateRequest) {
            Request? request = await _context.Requests.FirstOrDefaultAsync(r => r.ID == updateRequest.ID);

            if (request == null || request.UserId != GetUserId(_contextAccessor))
                throw new KeyNotFoundException(IdNotFoundMessage("request", updateRequest.ID));

            if (request.EmployeeId != updateRequest.EmployeeId)
                throw new AppException(string.Format(
                    "employeeId = {0} does not match that of stored value = {1}",
                    updateRequest.EmployeeId, request.EmployeeId));


            request.Start = updateRequest.Start;
            request.End = updateRequest.End;
            request.Description = updateRequest.Description;

            await _context.SaveChangesAsync();

            return _mapper.Map<GetRequestDto>(request);
        }
    }
}
