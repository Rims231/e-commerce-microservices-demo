using eCommerce.Order.Application.DTOs;
using eCommerce.Order.Application.Interfaces;
using eCommerce.SharedLibrary.Logs;
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
            try
            {
                var pipeline = resiliencePipeline.GetPipeline("my-pipeline");

                // Safely get product — handle 404 without throwing
                ProductDTO? product = null;
                try
                {
                    product = await pipeline.ExecuteAsync(async token =>
                        await httpClient.GetFromJsonAsync<ProductDTO>
                        ($"api/products/{orderDTO.ProductId}", token));
                }
                catch (HttpRequestException)
                {
                    return new Response(false, "Product not found");
                }

                if (product is null)
                    return new Response(false, "Product not found");

                if (product.Quantity < orderDTO.PurchaseQuantity)
                    return new Response(false, "Insufficient product quantity");

                var getOrder = await orderInterface.GetByAsync(o =>
                    o.ProductId == orderDTO.ProductId &&
                    o.ClientId == orderDTO.ClientId);

                if (getOrder is not null)
                    return new Response(false, "Order already placed");

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
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new Response(false, ex.Message);
            }
        }

        public async Task<Response> UpdateOrder(OrderDTO orderDTO)
        {
            var getOrder = await orderInterface.GetByIdAsync(orderDTO.Id);
            if (getOrder is null)
                return new Response(false, "Order not found");

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

            var pipeline = resiliencePipeline.GetPipeline("my-pipeline");
            var orderDetailsList = new List<OrderDetailsDTO>();

            foreach (var order in orders)
            {
                ProductDTO? product = null;
                try
                {
                    product = await pipeline.ExecuteAsync(async token =>
                        await httpClient.GetFromJsonAsync<ProductDTO>
                        ($"api/products/{order.ProductId}", token));
                }
                catch (HttpRequestException) { }

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

        public async Task<AppUserDTO> GetAppUser(int userId)
        {
            var pipeline = resiliencePipeline.GetPipeline("my-pipeline");

            var user = await pipeline.ExecuteAsync(async token =>
                await httpClient.GetFromJsonAsync<AppUserDTO>
                ($"api/Auth/{userId}", token));

            return user!;
        }
    }
}