using ECom.Core.Entities;
using ECom.Core.Entities.Order;
using ECom.Core.Entities.Product;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ECom.Infrastructure.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<OrderItems> OrderItems { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Seed Roles
            modelBuilder.Entity<IdentityRole>().HasData(
                // Seeding Data With Add ConcurrencyStamp
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "1" },
                new IdentityRole { Id = "2", Name = "Customer", NormalizedName = "CUSTOMER", ConcurrencyStamp = "2" },
                new IdentityRole { Id = "3", Name = "Vendor", NormalizedName = "VENDOR", ConcurrencyStamp = "3" }
                );

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 4, Name = "Electronics", Description = "Electornics Description" },
                new Category { Id = 2, Name = "Clothing", Description = "Cloths Description" },
                new Category { Id = 3, Name = "Books", Description = "Books Description" }
            );

            //// Seed Delivery Methods
            //modelBuilder.Entity<DeliveryMethod>().HasData(
            //    new DeliveryMethod { Id = 3, Name = "Standard", Price = 50.00m, DeliveryTime = "5-7 Days", Description = "Standard Delivery" },
            //    new DeliveryMethod { Id = 4, Name = "Express", Price = 75.00m, DeliveryTime = "2-3 Days", Description = "Express Delivery" },
            //    new DeliveryMethod { Id = 5, Name = "Same Day", Price = 80.00m, DeliveryTime = "Same Day", Description = "Same Day Delivery"}
            //    );
        }
    }
}
