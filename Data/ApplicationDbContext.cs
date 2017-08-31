using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data
{
 
//   public class ApplicationDbContextFactory : IDbContextFactory<ApplicationDbContext>
//     {
//         public ApplicationDbContext Create(DbContextFactoryOptions options)
//         {
//             var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//             optionsBuilder.UseSqlite("Filename=./appdata.db");

//             return new ApplicationDbContext(optionsBuilder.Options);
//         }
//     }

   public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Filename=./appdata.db");
        }

    }
}
