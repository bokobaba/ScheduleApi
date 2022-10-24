using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScheduleApi.Data;
using ScheduleApi.Dtos.EmployeeDtos;
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
            IEnumerable<Employee> employees = await _context.Employees
                .Where(e => e.UserId == GetUserId(_contextAccessor))
                .ToListAsync();

            return employees.Select(e => _mapper.Map<GetEmployeeDto>(e)).ToList();
        }

        public async Task<IEnumerable<GetEmployeeInfoDto>> GetAllEmployeeInfo() {
            string userId = GetUserId(_contextAccessor);
            IEnumerable<Employee> employees = await _context.Employees
                .Where(e => e.UserId == userId)
                .Include(e => e.Requests)
                .Include(e => e.Availability)
                .Include(e => e.Schedules)
                .ToListAsync();

            return employees.Select(e => _mapper.Map<GetEmployeeInfoDto>(e)).ToList();
        }

        public async Task<GetEmployeeInfoDto> GetEmployeeInfo(int id) {
            string userId = GetUserId(_contextAccessor);
            Employee? employee = await _context.Employees
                .Include(e => e.Requests)
                .Include(e => e.Availability)
                .Include(e => e.Schedules)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.EmployeeId == id);

            if (employee == null) {
                throw new KeyNotFoundException(IdNotFoundMessage("employee", id));
            }

            return _mapper.Map<GetEmployeeInfoDto>(employee);
        }

        public async Task<GetEmployeeDto> GetEmployeeById(int id) {
            string userId = GetUserId(_contextAccessor);
            Employee? employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId && e.EmployeeId == id);

            if (employee == null) {
                throw new KeyNotFoundException(IdNotFoundMessage("employee", id));
            }

            return _mapper.Map<GetEmployeeDto>(employee);
        }

        public async Task<GetEmployeeDto> UpdateEmployee(UpdateEmployeeDto updateEmployee) {
            string userId = GetUserId(_contextAccessor);
            Employee? employee = await _context.Employees
                .Include(e => e.Requests)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.EmployeeId == updateEmployee.EmployeeId);

            if (employee == null) {
                throw new KeyNotFoundException(IdNotFoundMessage("employee", updateEmployee.EmployeeId));
            }

            employee.Name = updateEmployee.Name;

            await _context.SaveChangesAsync();

            return _mapper.Map<GetEmployeeDto>(employee);
        }

        public async Task DeleteEmployee(int id) {
            string userId = GetUserId(_contextAccessor);
            Employee? toDelete = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId && e.EmployeeId == id);

            if (toDelete == null) {
                throw new KeyNotFoundException(IdNotFoundMessage("employee", id));
            }

            _context.Employees.Remove(toDelete);
            await _context.SaveChangesAsync();
        }
    }
}
