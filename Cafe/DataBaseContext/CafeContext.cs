using Cafe.Model;
using Microsoft.EntityFrameworkCore;

namespace Cafe.DataBaseContext
{
    public class CafeContext : DbContext
    {
        public CafeContext(DbContextOptions<CafeContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Waiter> Waiters { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.MenuItemId });

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany(mi => mi.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId);
        }
    }
}
