using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Booking.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Booking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();

            // Seed data


            using (var scope = host.Services.CreateScope())  // create a IServiceScope
            {
                var services = scope.ServiceProvider;
                // Get database context
                var context = services.GetRequiredService<ApplicationDbContext>();
                // Apply pending migrations. Create database if not existing.
                context.Database.Migrate();
                // Get configuration 
                var config = services.GetRequiredService<IConfiguration>();

                //dotnet user secrets set "AdminPW" "password"

                var adminPW = config["AdminPW"];

                try
                {
                    SeedData.InitializeAsync(services, adminPW).Wait();   // wait for task to complete
                }
                catch (Exception e)
                {
                    // Get logger. Category is class Booking.Program
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e.Message, "Seed failed");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
