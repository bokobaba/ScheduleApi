using Microsoft.EntityFrameworkCore;
using ScheduleApi.Models;
using static ScheduleApi.Utils.Utils;

namespace ScheduleApi.Data {
    public class ScheduleDbContext : DbContext {
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRefreshToken> RefreshTokens { get; set; }

        public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Employee>()
                .HasMany(e => e.Requests)
                .WithOne(r => r.Employee)
                .HasPrincipalKey(e => e.EmployeeId)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Employee>()
                .HasMany(e => e.Schedules)
                .WithOne(s => s.Employee)
                .HasPrincipalKey(e => e.EmployeeId)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Schedule>()
                .HasIndex(s => new { s.Week, s.Year, s.EmployeeId, s.Day }).IsUnique();
        }

        public async Task<Employee> CheckEmployeeValid(int employeeId) {
            Employee? employee = await Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            if (employee == null)
                throw new KeyNotFoundException(IdNotFoundMessage("employee", employeeId));
            return employee;
        }

        public async Task CheckUserValid(string source, int requestId, int employeeId, IHttpContextAccessor ctx) {
            Employee employee = await CheckEmployeeValid(employeeId);
            if (employee.UserId != GetUserId(ctx)) {
                throw new KeyNotFoundException(IdNotFoundMessage(source, requestId));
            }
        }

        public async Task CheckEmployeeUserValid(int employeeId, IHttpContextAccessor ctx) {
            Employee employee = await CheckEmployeeValid(employeeId);
            if (employee.UserId != GetUserId(ctx)) {
                throw new KeyNotFoundException(IdNotFoundMessage("employee", employeeId));
            }
        }
    }
}
