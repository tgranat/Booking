using System;
using System.Collections.Generic;
using System.Text;
using Booking.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Booking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<GymClass> GymClasses { get; set; }
        public DbSet<ApplicationUserGymClass> ApplicationUserGymClasses { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //Tyvärr finns ännu inte funktionaliteten att definiera upp en komposit nyckel med hjälp av data
        //annotations.Så vi får använda oss av fluent api för att åstadkomma det.
        //Vi använder oss av en override av OnModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Fix composite key
            builder.Entity<ApplicationUserGymClass>().HasKey(k => new { k.ApplicationUserId, k.GymClassId });

            // Queryfilter
            builder.Entity<GymClass>().HasQueryFilter(g => g.StartDate > DateTime.Now);
        }
    }
}
