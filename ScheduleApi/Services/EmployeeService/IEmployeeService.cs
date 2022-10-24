using ScheduleApi.Dtos.EmployeeDtos;

namespace ScheduleApi.Services.EmployeeService {
    public interface IEmployeeService {

        Task<IEnumerable<GetEmployeeDto>> GetAllEmployees();
        Task<GetEmployeeDto> GetEmployeeById(int id);
        Task<GetEmployeeDto> AddEmployee(AddEmployeeDto employee);
        Task<GetEmployeeDto> UpdateEmployee(UpdateEmployeeDto employee);
        Task DeleteEmployee(int id);
        Task<IEnumerable<GetEmployeeInfoDto>> GetAllEmployeeInfo();
        Task<GetEmployeeInfoDto> GetEmployeeInfo(int id);
    }
}
