using CourierAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourierAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Courier> Couriers { get; set; }
        public DbSet<Dispatcher> Dispatchers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<RouteElement> RouteElements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //seed admin role
            var role = new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
            };
            builder.Entity<IdentityRole>().HasData(role);

            //seed admin user
            var appUser = new IdentityUser
            {
                UserName = "admin",
                NormalizedUserName = "ADMIN",
            };

            appUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(appUser, "Admin1234_?");
            builder.Entity<IdentityUser>().HasData(appUser);

            //set role do admin
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = role.Id,
                UserId = appUser.Id,
            });

            //seed other roles
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "Courier",
                NormalizedName = "COURIER",
            });
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "Dispatcher",
                NormalizedName = "DISPATCHER",
            });
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "Customer",
                NormalizedName = "CUSTOMER",
            });

            builder.Entity<Courier>(entity => { entity.ToTable("Couriers"); });
            builder.Entity<Dispatcher>(entity => { entity.ToTable("Dispatchers"); });
            builder.Entity<Customer>(entity => { entity.ToTable("Customers"); });
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
