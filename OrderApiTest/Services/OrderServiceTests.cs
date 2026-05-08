using eCommerce.Order.Application.DTOs;
using eCommerce.Order.Application.Interfaces;
using eCommerce.Order.Application.Services;
using eCommerce.SharedLibrary.Response;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using OrderEntity = eCommerce.Order.Domain.Entities.Order;

namespace UnitTest.OrderApi
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrder> mockOrderInterface;
        private readonly Mock<HttpMessageHandler> mockHttpHandler;
        private readonly Mock<ILogger<OrderService>> mockLogger;
        private readonly ResiliencePipelineProvider<string> resiliencePipeline;
        private readonly HttpClient httpClient;
        private readonly OrderService orderService;

        public OrderServiceTests()
        {
            mockOrderInterface = new Mock<IOrder>();
            mockHttpHandler = new Mock<HttpMessageHandler>();
            mockLogger = new Mock<ILogger<OrderService>>();

            // Setup HttpClient with mock handler
            httpClient = new HttpClient(mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:5056/")
            };

            // Setup Resilience Pipeline
            var registry = new ResiliencePipelineRegistry<string>();
            registry.TryAddBuilder("my-pipeline", (builder, _) =>
            {
                builder.AddRetry(new Polly.Retry.RetryStrategyOptions
                {
                    MaxRetryAttempts = 1,
                    Delay = TimeSpan.Zero
                });
            });
            resiliencePipeline = registry;

            orderService = new OrderService(
                mockOrderInterface.Object,
                httpClient,
                resiliencePipeline,
                mockLogger.Object);
        }

        private void SetupHttpProduct(ProductDTO? product)
        {
            var response = product is null
                ? new HttpResponseMessage(HttpStatusCode.NotFound)
                : new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(product),
                        System.Text.Encoding.UTF8,
                        "application/json")
                };

            mockHttpHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);
        }

        // PLACE ORDER
        [Fact]
        public async Task PlaceOrder_ProductAvailable_ReturnsSuccess()
        {
            // Arrange
            var orderDTO = new OrderDTO(0, 1, 1, 2, DateTime.UtcNow);
            var product = new ProductDTO(1, "iPhone 15", 999.99m, 50);

            SetupHttpProduct(product);

            mockOrderInterface.Setup(x => x.GetByAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderEntity, bool>>>()))
                .ReturnsAsync((OrderEntity?)null);

            mockOrderInterface.Setup(x => x.CreateAsync(It.IsAny<OrderEntity>()))
                .ReturnsAsync(new Response(true, "Order placed successfully"));

            // Act
            var result = await orderService.PlaceOrder(orderDTO);

            // Assert
            result.flag.Should().BeTrue();
            result.message.Should().Be("Order placed successfully");
        }

        [Fact]
        public async Task PlaceOrder_ProductNotFound_ReturnsFailure()
        {
            // Arrange
            var orderDTO = new OrderDTO(0, 1, 1, 2, DateTime.UtcNow);
            SetupHttpProduct(null);

            // Act
            var result = await orderService.PlaceOrder(orderDTO);

            // Assert
            result.flag.Should().BeFalse();
            result.message.Should().Be("Product not found");
        }

        [Fact]
        public async Task PlaceOrder_InsufficientQuantity_ReturnsFailure()
        {
            // Arrange
            var orderDTO = new OrderDTO(0, 1, 1, 100, DateTime.UtcNow);
            var product = new ProductDTO(1, "iPhone 15", 999.99m, 10);

            SetupHttpProduct(product);

            // Act
            var result = await orderService.PlaceOrder(orderDTO);

            // Assert
            result.flag.Should().BeFalse();
            result.message.Should().Be("Insufficient product quantity");
        }

        [Fact]
        public async Task PlaceOrder_OrderAlreadyPlaced_ReturnsFailure()
        {
            // Arrange
            var orderDTO = new OrderDTO(0, 1, 1, 2, DateTime.UtcNow);
            var product = new ProductDTO(1, "iPhone 15", 999.99m, 50);
            var existingOrder = new OrderEntity
            {
                Id = 1,
                ProductId = 1,
                ClientId = 1,
                PurchaseQuantity = 2,
                OrderedDate = DateTime.UtcNow
            };

            SetupHttpProduct(product);

            mockOrderInterface.Setup(x => x.GetByAsync(It.IsAny<System.Linq.Expressions.Expression<Func<OrderEntity, bool>>>()))
                .ReturnsAsync(existingOrder);

            // Act
            var result = await orderService.PlaceOrder(orderDTO);

            // Assert
            result.flag.Should().BeFalse();
            result.message.Should().Be("Order already placed");
        }

        // UPDATE ORDER
        [Fact]
        public async Task UpdateOrder_ExistingOrder_ReturnsSuccess()
        {
            // Arrange
            var orderDTO = new OrderDTO(1, 1, 1, 5, DateTime.UtcNow);
            var existingOrder = new OrderEntity
            {
                Id = 1,
                ProductId = 1,
                ClientId = 1,
                PurchaseQuantity = 2,
                OrderedDate = DateTime.UtcNow
            };

            mockOrderInterface.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingOrder);
            mockOrderInterface.Setup(x => x.UpdateAsync(It.IsAny<OrderEntity>()))
                .ReturnsAsync(new Response(true, "Order updated successfully"));

            // Act
            var result = await orderService.UpdateOrder(orderDTO);

            // Assert
            result.flag.Should().BeTrue();
            result.message.Should().Be("Order updated successfully");
        }

        [Fact]
        public async Task UpdateOrder_NonExistingOrder_ReturnsFailure()
        {
            // Arrange
            var orderDTO = new OrderDTO(99, 1, 1, 2, DateTime.UtcNow);
            mockOrderInterface.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((OrderEntity?)null);

            // Act
            var result = await orderService.UpdateOrder(orderDTO);

            // Assert
            result.flag.Should().BeFalse();
            result.message.Should().Be("Order not found");
        }

        // DELETE ORDER
        [Fact]
        public async Task DeleteOrder_ExistingOrder_ReturnsSuccess()
        {
            // Arrange
            var orderDTO = new OrderDTO(1, 1, 1, 2, DateTime.UtcNow);
            var existingOrder = new OrderEntity
            {
                Id = 1,
                ProductId = 1,
                ClientId = 1,
                PurchaseQuantity = 2,
                OrderedDate = DateTime.UtcNow
            };

            mockOrderInterface.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingOrder);
            mockOrderInterface.Setup(x => x.DeleteAsync(It.IsAny<OrderEntity>()))
                .ReturnsAsync(new Response(true, "Order deleted successfully"));

            // Act
            var result = await orderService.DeleteOrder(orderDTO);

            // Assert
            result.flag.Should().BeTrue();
            result.message.Should().Be("Order deleted successfully");
        }

        [Fact]
        public async Task DeleteOrder_NonExistingOrder_ReturnsFailure()
        {
            // Arrange
            var orderDTO = new OrderDTO(99, 1, 1, 2, DateTime.UtcNow);
            mockOrderInterface.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((OrderEntity?)null);

            // Act
            var result = await orderService.DeleteOrder(orderDTO);

            // Assert
            result.flag.Should().BeFalse();
            result.message.Should().Be("Order not found");
        }

        // GET ORDERS
        [Fact]
        public async Task GetOrders_OrdersExist_ReturnsOrderDTOs()
        {
            // Arrange
            var orders = new List<OrderEntity>
            {
                new() { Id = 1, ProductId = 1, ClientId = 1, PurchaseQuantity = 2, OrderedDate = DateTime.UtcNow },
                new() { Id = 2, ProductId = 2, ClientId = 2, PurchaseQuantity = 3, OrderedDate = DateTime.UtcNow }
            };
            mockOrderInterface.Setup(x => x.GetAllAsync()).ReturnsAsync(orders);

            // Act
            var result = await orderService.GetOrders();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        // GET ORDER BY ID
        [Fact]
        public async Task GetOrder_ExistingOrder_ReturnsOrderDTO()
        {
            // Arrange
            var order = new OrderEntity
            {
                Id = 1,
                ProductId = 1,
                ClientId = 1,
                PurchaseQuantity = 2,
                OrderedDate = DateTime.UtcNow
            };
            mockOrderInterface.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);

            // Act
            var result = await orderService.GetOrder(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.ProductId.Should().Be(1);
        }
    }
}