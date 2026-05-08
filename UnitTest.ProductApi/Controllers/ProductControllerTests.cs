using eCommerce.Product.Domain.Interfaces;
using eCommerce.Product.Presentation.Controllers;
using eCommerce.SharedLibrary.Response;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductEntity = eCommerce.Product.Domain.Entities.Product;

namespace UnitTest.ProductApi
{
    public class ProductControllerTests
    {
        private readonly Mock<IProduct> mockProductInterface;
        private readonly ProductsController controller;

        public ProductControllerTests()
        {
            mockProductInterface = new Mock<IProduct>();
            controller = new ProductsController(mockProductInterface.Object);
        }

        // GET ALL
        [Fact]
        public async Task GetAll_ProductsExist_ReturnsOk()
        {
            // Arrange
            var products = new List<ProductEntity>
            {
                new() { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 },
                new() { Id = 2, Name = "Samsung S24", Price = 899.99m, Quantity = 30 }
            };
            mockProductInterface.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetAll_NoProducts_ReturnsNotFound()
        {
            // Arrange
            mockProductInterface.Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ProductEntity>());

            // Act
            var result = await controller.GetAll();

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
        }

        // GET BY ID
        [Fact]
        public async Task GetById_ExistingProduct_ReturnsOk()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };
            mockProductInterface.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await controller.GetById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetById_NonExistingProduct_ReturnsNotFound()
        {
            // Arrange
            mockProductInterface.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((ProductEntity?)null);

            // Act
            var result = await controller.GetById(99);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
        }

        // CREATE
        [Fact]
        public async Task Create_ValidProduct_ReturnsOk()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };
            mockProductInterface.Setup(x => x.CreateAsync(product))
                .ReturnsAsync(new Response(true, "iPhone 15 created successfully"));

            // Act
            var result = await controller.Create(product);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Create_InvalidProduct_ReturnsBadRequest()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };
            mockProductInterface.Setup(x => x.CreateAsync(product))
                .ReturnsAsync(new Response(false, "Error occurred creating product"));

            // Act
            var result = await controller.Create(product);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        // UPDATE
        [Fact]
        public async Task Update_ExistingProduct_ReturnsOk()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 16", Price = 1099.99m, Quantity = 30 };
            mockProductInterface.Setup(x => x.UpdateAsync(product))
                .ReturnsAsync(new Response(true, "iPhone 16 updated successfully"));

            // Act
            var result = await controller.Update(product);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Update_NonExistingProduct_ReturnsBadRequest()
        {
            // Arrange
            var product = new ProductEntity { Id = 99, Name = "Ghost", Price = 0, Quantity = 0 };
            mockProductInterface.Setup(x => x.UpdateAsync(product))
                .ReturnsAsync(new Response(false, "Product not found"));

            // Act
            var result = await controller.Update(product);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        // DELETE
        [Fact]
        public async Task Delete_ExistingProduct_ReturnsOk()
        {
            // Arrange
            var product = new ProductEntity { Id = 1, Name = "iPhone 15", Price = 999.99m, Quantity = 50 };
            mockProductInterface.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);
            mockProductInterface.Setup(x => x.DeleteAsync(product))
                .ReturnsAsync(new Response(true, "iPhone 15 deleted successfully"));

            // Act
            var result = await controller.Delete(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Delete_NonExistingProduct_ReturnsNotFound()
        {
            // Arrange
            mockProductInterface.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((ProductEntity?)null);

            // Act
            var result = await controller.Delete(99);

            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
        }
    }
}