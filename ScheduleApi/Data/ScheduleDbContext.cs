using Microsoft.EntityFrameworkCore;
using ScheduleApi.Models;

namespace ScheduleApi.Data {
    public class ScheduleDbContext : DbContext {
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Request> Requests { get; set; }

        public ScheduleDbContext(DbContextOptions<ScheduleDbContext> options) : base(options) { }
    }
}
