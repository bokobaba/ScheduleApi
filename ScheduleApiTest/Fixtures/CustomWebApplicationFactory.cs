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
        public static bool init = false;

        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            if (init) return;
            init = true;
            builder.ConfigureServices(services => {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ScheduleDbContext>));

                services.Remove(descriptor);

                IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

                services.AddDbContext<ScheduleDbContext>(options => {
                    options.UseSqlServer(configuration.GetConnectionString("dbTestConnection"));
                });

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
