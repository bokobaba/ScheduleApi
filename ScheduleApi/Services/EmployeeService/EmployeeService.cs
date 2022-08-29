using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleApi.Data;
using ScheduleApi.Dtos.EmployeeDtos;
using ScheduleApi.Exceptions;
using ScheduleApi.Models;
using static ScheduleApi.Utils.Utils;

namespace ScheduleApi.Services.EmployeeService {
    public class EmployeeService : IEmployeeService {
        private readonly ScheduleDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public EmployeeService(ScheduleDbContext context, IMapper mapper, IHttpContextAccessor contextAccessor) {
            _context = context;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<GetEmployeeDto> AddEmployee(AddEmployeeDto newEmployee) {
            Employee employee = _mapper.Map<Employee>(newEmployee);
            employee.UserId = GetUserId(_contextAccessor);

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return _mapper.Map<GetEmployeeDto>(employee);
        }

        public async Task<IEnumerable<GetEmployeeDto>?> GetAllEmployees() {
            IEnumerable<Employee>? employees = await _context.Employees
                .Include(e => e.Requests)
                .Where(e => e.User.ID == GetUserId(_contextAccessor))
                .ToListAsync();

            return employees == null ? null : employees.Select(e => _mapper.Map<GetEmployeeDto>(e)).ToList();
        }

        public async Task<GetEmployeeDto> GetEmployeeById(int id) {
            Employee? employee = await _context.Employees
                .Include(_e => _e.Requests)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null || employee.UserId != GetUserId(_contextAccessor)) {
                throw new KeyNotFoundException(IdNotFoundMessage("employee", id));
            }

            return _mapper.Map<GetEmployeeDto>(employee);
        }

        public async Task<GetEmployeeDto> UpdateEmployee(UpdateEmployeeDto updateEmployee) {
            Employee? employee = await _context.Employees
                .Include(e => e.Requests)
                .FirstOrDefaultAsync(e => e.EmployeeId == updateEmployee.EmployeeId);

            if (employee == null || employee.UserId != GetUserId(_contextAccessor)) {
                throw new KeyNotFoundException(IdNotFoundMessage("employee", updateEmployee.EmployeeId));
            }

            employee.Name = updateEmployee.Name;

            if (employee.Requests != null)
                _context.Requests.RemoveRange(employee.Requests);
            employee.Requests = updateEmployee.Requests == null ? null : 
                updateEmployee.Requests.Select(r => _mapper.Map<Request>(r)).ToList();

            await _context.SaveChangesAsync();

            return _mapper.Map<GetEmployeeDto>(employee);
        }

        public async Task DeleteEmployee(int id) {
            Employee? toDelete = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (toDelete == null || toDelete.UserId != GetUserId(_contextAccessor)) {
                throw new KeyNotFoundException(IdNotFoundMessage("employee", id));
            }

            _context.Employees.Remove(toDelete);
            await _context.SaveChangesAsync();
        }
    }
}
