using CourierAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourierAPI.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Courier> Couriers { get; set; }
    public DbSet<Dispatcher> Dispatchers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<RouteElement> RouteElements { get; set; }
    public DbSet<PriceList> PriceList { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //seed admin role
        var role = new IdentityRole
        {
            Id = "00000000-0000-0000-0000-000000000001",
            Name = "Admin",
            NormalizedName = "ADMIN",
        };
        builder.Entity<IdentityRole>().HasData(role);

        //seed admin user
        var appUser = new IdentityUser
        {
            Id = "00000000-0000-0000-0000-000000000001",
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@admin.com",
            EmailConfirmed = true,
        };

        appUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(appUser, "Admin1234_?");
        builder.Entity<IdentityUser>().HasData(appUser);

        //set role to admin
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

        builder.Entity<PriceList>().HasData(new PriceList
        {
            Id = 1,
            VerySmallSize = 5,
            SmallSize = 10,
            MediumSize = 15,
            LargeSize = 20,
            LightWeight = 3.99f,
            MediumWeight = 6.99f,
            HeavyWeight = 10.99f,
        });

        builder.Entity<Courier>(entity => { entity.ToTable("Couriers"); });
        builder.Entity<Dispatcher>(entity => { entity.ToTable("Dispatchers"); });
        builder.Entity<Customer>(entity => { entity.ToTable("Customers"); });
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
