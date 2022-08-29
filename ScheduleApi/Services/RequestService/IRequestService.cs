using ScheduleApi.Dtos.EmployeeDtos;
using ScheduleApi.Dtos.RequestDtos;

namespace ScheduleApi.Services.RequestService {
    public interface IRequestService {
        Task<GetRequestDto> AddRequest(AddRequestDto request);
        Task<IEnumerable<GetRequestDto>?> GetAllRequests();
        Task<GetRequestDto> GetReqeustById(int id);
        Task<IEnumerable<GetRequestDto>?> GetReqeustsByEmployeeId(int employeeId);
        Task<GetRequestDto> UpdateRequest(UpdateRequestDto request);
        Task DeleteRequest(int id);
    }
}
