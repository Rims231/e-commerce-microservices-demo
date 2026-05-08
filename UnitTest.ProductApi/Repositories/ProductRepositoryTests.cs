using eCommerce.Product.Infrastructure.Data;
using eCommerce.Product.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductEntity = eCommerce.Product.Domain.Entities.Product;

namespace UnitTest.ProductApi
{
    public class ProductRepositoryTests
    {
        private readonly ProductDbContext context;
        private readonly ProductRepository repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            context = new ProductDbContext(options);
            repository = new ProductRepository(context);
        }
       
      
        [Fact]
        public async Task GetAllAsync_ProductsExist_ReturnsAllProducts()
        {
           
            await context.Products.AddRangeAsync(new List<ProductEntity>
            {
                new() { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 },
                new() { Id = 2, Name = "Samsung S24", Price = 899.99m, Quantity = 30 }
            });
            await context.SaveChangesAsync();

          
            var result = await repository.GetAllAsync();

          
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetAllAsync_NoProducts_ReturnsEmptyList()
        {
           
            var result = await repository.GetAllAsync();

           
            result.Should().BeEmpty();
        }

       
        [Fact]
        public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
        {
           
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var result = await repository.GetByIdAsync(1);

            
            result.Should().NotBeNull();
            result!.Name.Should().Be("iPhone 15");
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingProduct_ReturnsNull()
        {
            
            var result = await repository.GetByIdAsync(99);

           
            result.Should().BeNull();
        }
    }
}