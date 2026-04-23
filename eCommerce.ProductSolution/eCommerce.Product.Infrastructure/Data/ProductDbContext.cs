using Microsoft.EntityFrameworkCore;

namespace eCommerce.Product.Infrastructure.Data
{
    public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
    {
        public DbSet<Domain.Entities.Product> Products { get; set; }
    }
}