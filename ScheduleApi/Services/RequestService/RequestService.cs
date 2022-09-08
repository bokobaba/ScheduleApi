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

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return _mapper.Map<GetRequestDto>(request);
        }

        public async Task DeleteRequest(int id) {
            Request? toDelete = await _context.Requests.FirstOrDefaultAsync(r => r.ID == id);

            if (toDelete == null)
                throw new KeyNotFoundException(IdNotFoundMessage("request", id));

            await _context.CheckUserValid("request", id, toDelete.EmployeeId, _contextAccessor);

            _context.Requests.Remove(toDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetRequestDto>?> GetAllRequests() {
            IEnumerable<Request>? requests = await _context.Requests
                .Include(r => r.Employee)
                .Where(r => r.Employee.UserId == GetUserId(_contextAccessor))
                .ToListAsync();

            return requests?.Select(r => _mapper.Map<GetRequestDto>(r)).ToList();
        }

        public async Task<GetRequestDto> GetReqeustById(int id) {
            Request? request = await _context.Requests.FirstOrDefaultAsync(r => r.ID == id);
            if (request == null)
                throw new KeyNotFoundException(IdNotFoundMessage("request", id));

            await _context.CheckUserValid("request", id, request.EmployeeId, _contextAccessor);

            return _mapper.Map<GetRequestDto>(request);
        }

        public async Task<IEnumerable<GetRequestDto>?> GetReqeustsByEmployeeId(int employeeId) {
            await _context.CheckEmployeeUserValid(employeeId, _contextAccessor);

            IEnumerable<Request>? requests = await _context.Requests
                .Where(r => r.EmployeeId == employeeId)
                .ToListAsync();

            return requests?.Select(r => _mapper.Map<GetRequestDto>(r)).ToList();
        }

        public async Task<GetRequestDto> UpdateRequest(UpdateRequestDto updateRequest) {
            await _context.CheckEmployeeUserValid(updateRequest.EmployeeId, _contextAccessor);

            Request? request = await _context.Requests.FirstOrDefaultAsync(r => r.ID == updateRequest.ID);

            if (request == null)
                throw new KeyNotFoundException(IdNotFoundMessage("request", updateRequest.ID));

            if (request.EmployeeId != updateRequest.EmployeeId)
                throw new AppException(string.Format(
                    "employeeId = {0} does not match that of stored value = {1}",
                    updateRequest.EmployeeId, request.EmployeeId));


            request.Start = updateRequest.Start;
            request.End = updateRequest.End;

            await _context.SaveChangesAsync();

            return _mapper.Map<GetRequestDto>(request);
        }
    }
}
