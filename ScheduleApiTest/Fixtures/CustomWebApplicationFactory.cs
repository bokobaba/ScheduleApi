using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using ScheduleApi.Data;
using ScheduleApiTest.Helpers;
using ScheduleApiTest.Models;

namespace ScheduleApiTest.Fixtures {
    public class CustomWebApplicationFactory<Program>
    : WebApplicationFactory<Program> where Program : class {

        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            builder.ConfigureServices(services => {
                //var descriptor = services.SingleOrDefault(
                //    d => d.ServiceType == typeof(DbContextOptions<ScheduleDbContext>));

                //services.Remove(descriptor);

                //services.AddDbContext<ScheduleDbContext>(options => {
                //    options.UseSqlServer("server=(localdb)\\MSSQLLocalDB;database=ScheduleApiDb;");
                //});

                //services.AddDbContext<ScheduleDbContext>(options => {
                //    options.UseInMemoryDatabase("InMemoryDbForTesting");
                //});

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope()) {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ScheduleDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<Program>>>();

                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    try {
                        Utilities.InitializeDbForTests(db);
                    } catch (Exception ex) {
                        logger.LogError(ex, "An error occurred seeding the " +
                            "database. Error: {Message}", ex.Message);
                    }
                }
            });
        }
    }
}
