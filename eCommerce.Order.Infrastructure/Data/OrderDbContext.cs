using Microsoft.EntityFrameworkCore;
using OrderEntity = eCommerce.Order.Domain.Entities.Order;

namespace eCommerce.Order.Infrastructure.Data
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<OrderEntity> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderEntity>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.ProductId)
                    .IsRequired();

                entity.Property(o => o.ClientId)
                    .IsRequired();

                entity.Property(o => o.PurchaseQuantity)
                    .IsRequired();

                entity.Property(o => o.OrderedDate)
                    .IsRequired();
            });
        }
    }
}