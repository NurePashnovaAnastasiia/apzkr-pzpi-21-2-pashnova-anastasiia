using LightServeWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LightServeWebAPI.Database

{
    public class LightServeContext : DbContext
    {
        public LightServeContext(DbContextOptions<LightServeContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dish>()
                .HasMany(d => d.OrderDetails)
                .WithOne(od => od.Dish)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Dishes)
                .WithOne(d => d.Menu)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Cafe> Cafes { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Dish> Dishes { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }

        public DbSet<Owner> Owners { get; set; }

        public DbSet<Table> Tables { get; set; }

        public DbSet<Worker> Workers { get; set; }
    }
}
