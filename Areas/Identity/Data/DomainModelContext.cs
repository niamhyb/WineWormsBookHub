using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DomainModel.Data
{
    //remove identityUser
    public class DomainModelContext : IdentityDbContext<ApplicationUser>
    {
        public DomainModelContext(DbContextOptions<DomainModelContext> options)
            : base(options)
        {
        }

        //new line
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Book> BookTable { get; set; }
        public DbSet<Catalogue> catalogues { get; set; }

        public DbSet<Reservation> reservations { get; set; }
        public DbSet<Loan> loans { get; set; }
        //public DbSet<Test> tests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>();
                //.Property("Discriminator")
                //.HasMaxLength(200);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }


    }
}
