using eCommerce.Order.Application.DTOs;
using eCommerce.Order.Application.Interfaces;
using eCommerce.SharedLibrary.Response;
using Microsoft.Extensions.Logging;
using Polly.Registry;
using System.Net.Http.Json;
using OrderEntity = eCommerce.Order.Domain.Entities.Order;

namespace eCommerce.Order.Application.Services
{
    public class OrderService(
        IOrder orderInterface,
        HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline,
        ILogger<OrderService> logger) : IOrderService
    {
        public async Task<Response> PlaceOrder(OrderDTO orderDTO)
        {
            // Check product availability via Product API
            var pipeline = resiliencePipeline.GetPipeline("my-pipeline");

            var product = await pipeline.ExecuteAsync(async token =>
                await httpClient.GetFromJsonAsync<ProductDTO>
                ($"api/products/{orderDTO.ProductId}", token));

            if (product is null)
                return new Response(false, "Product not found");

            if (product.Quantity < orderDTO.PurchaseQuantity)
                return new Response(false, "Insufficient product quantity");

            var getOrder = await orderInterface.GetByAsync(o =>
                o.ProductId == orderDTO.ProductId &&
                o.ClientId == orderDTO.ClientId);

            if (getOrder is not null)
                return new Response(false, "Order already placed");

            // Map DTO to entity
            var order = new OrderEntity()
            {
                ProductId = orderDTO.ProductId,
                ClientId = orderDTO.ClientId,
                PurchaseQuantity = orderDTO.PurchaseQuantity,
                OrderedDate = DateTime.UtcNow
            };

            var result = await orderInterface.CreateAsync(order);
            return result;
        }

        public async Task<Response> UpdateOrder(OrderDTO orderDTO)
        {
            var getOrder = await orderInterface.GetByIdAsync(orderDTO.Id);
            if (getOrder is null)
                return new Response(false, "Order not found");

            // Map DTO to entity
            getOrder.ProductId = orderDTO.ProductId;
            getOrder.ClientId = orderDTO.ClientId;
            getOrder.PurchaseQuantity = orderDTO.PurchaseQuantity;

            var result = await orderInterface.UpdateAsync(getOrder);
            return result;
        }

        public async Task<Response> DeleteOrder(OrderDTO orderDTO)
        {
            var getOrder = await orderInterface.GetByIdAsync(orderDTO.Id);
            if (getOrder is null)
                return new Response(false, "Order not found");

            var result = await orderInterface.DeleteAsync(getOrder);
            return result;
        }

        public async Task<IEnumerable<OrderDTO>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            return orders.Select(o => new OrderDTO(
                o.Id,
                o.ProductId,
                o.ClientId,
                o.PurchaseQuantity,
                o.OrderedDate));
        }

        public async Task<OrderDTO> GetOrder(int id)
        {
            var order = await orderInterface.GetByIdAsync(id);
            return new OrderDTO(
                order!.Id,
                order.ProductId,
                order.ClientId,
                order.PurchaseQuantity,
                order.OrderedDate);
        }

        public async Task<IEnumerable<OrderDetailsDTO>> GetOrdersByClientId(int clientId)
        {
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);

            // Enrich each order with product details via Product API
            var pipeline = resiliencePipeline.GetPipeline("my-pipeline");
            var orderDetailsList = new List<OrderDetailsDTO>();

            foreach (var order in orders)
            {
                var product = await pipeline.ExecuteAsync(async token =>
                    await httpClient.GetFromJsonAsync<ProductDTO>
                    ($"api/products/{order.ProductId}", token));

                orderDetailsList.Add(new OrderDetailsDTO(
                    order.Id,
                    order.ProductId,
                    order.ClientId,
                    ClientName: string.Empty,
                    Email: string.Empty,
                    Address: string.Empty,
                    TelephoneNumber: string.Empty,
                    ProductName: product?.Name ?? "Unknown",
                    ProductPrice: product?.Price ?? 0,
                    order.PurchaseQuantity,
                    order.OrderedDate));
            }

            return orderDetailsList;
        }
    }
}