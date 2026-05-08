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

        // GET ALL
        [Fact]
        public async Task GetAllAsync_ProductsExist_ReturnsAllProducts()
        {
            // Arrange
            await context.Products.AddRangeAsync(new List<ProductEntity>
            {
                new() { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 },
                new() { Id = 2, Name = "Samsung S24", Price = 899.99m, Quantity = 30 }
            });
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetAllAsync_NoProducts_ReturnsEmptyList()
        {
            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        // GET BY ID
        [Fact]
        public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("iPhone 15");
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingProduct_ReturnsNull()
        {
            // Act
            var result = await repository.GetByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }


        // CREATE
        [Fact]
        public async Task CreateAsync_ValidProduct_ReturnsSuccess()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };

            // Act
            var result = await repository.CreateAsync(product);

            // Assert
            result.flag.Should().BeTrue();
            result.message.Should().Contain("created successfully");
        }

        //[Fact]
        //public async Task CreateAsync_InvalidProduct_ReturnsFailure()
        //{
        //    // Arrange
        //    var product = new ProductEntity { Id = 0, Name = "", Price = 0, Quantity = 0 };

        //    // Act
        //    var result = await repository.CreateAsync(product);

        //    // Assert
        //    result.flag.Should().BeFalse();
        //} 

        [Fact]
        public async Task CreateAsync_DuplicateProduct_ReturnsFailure()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // Try to add same product again
            var duplicateProduct = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };

            // Act
            var result = await repository.CreateAsync(duplicateProduct);

            // Assert
            result.flag.Should().BeFalse();
        }

        // UPDATE
        [Fact]
        public async Task UpdateAsync_ExistingProduct_ReturnsSuccess()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var updatedProduct = new ProductEntity { Id = 1, Name = "iPhone 16", Price = 1099.99m, Quantity = 30 };

            // Act
            var result = await repository.UpdateAsync(updatedProduct);

            // Assert
            result.flag.Should().BeTrue();
            result.message.Should().Contain("updated successfully");
        }

        [Fact]
        public async Task UpdateAsync_NonExistingProduct_ReturnsFailure()
        {
            // Arrange
            var product = new ProductEntity { Id = 99, Name = "Ghost Product", Price = 0, Quantity = 0 };

            // Act
            var result = await repository.UpdateAsync(product);

            // Assert
            result.flag.Should().BeFalse();
            result.message.Should().Be("Product not found");
        }

        // DELETE
        [Fact]
        public async Task DeleteAsync_ExistingProduct_ReturnsSuccess()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeleteAsync(product);

            // Assert
            result.flag.Should().BeTrue();
            result.message.Should().Contain("deleted successfully");
        }

        [Fact]
        public async Task DeleteAsync_NonExistingProduct_ReturnsFailure()
        {
            // Arrange
            var product = new ProductEntity { Id = 99, Name = "Ghost Product", Price = 0, Quantity = 0 };

            // Act
            var result = await repository.DeleteAsync(product);

            // Assert
            result.flag.Should().BeFalse();
            result.message.Should().Be("Product not found");
        }
    }
}