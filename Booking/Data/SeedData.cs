using Bogus;
using Booking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Data
{
    public class SeedData
    {
        // Initialize database with bogus data. Called from Program.cs
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using (var context = new ApplicationDbContext(services.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (context.GymClass.Any()) return;

                var fake = new Faker("sv");
                var gymClasses = new List<GymClass>();

                for (int i = 0; i < 5; i++)
                {
                    var gymClass = new GymClass
                    {
                        Name = fake.Company.CatchPhrase(),
                        Description = fake.Hacker.Verb(),
                        Duration = new TimeSpan(0, 55, 0),
                        StartDate = DateTime.Now.AddDays(fake.Random.Int(-2, 2))
                    };
                    gymClasses.Add(gymClass);
                }

                await context.AddRangeAsync(gymClasses);
            }
        }
    }
}
