using eCommerce.Order.Application.DTOs;
using eCommerce.SharedLibrary.Response;

namespace eCommerce.Order.Application.Services
{
    public interface IOrderService
    {
        Task<Response> PlaceOrder(OrderDTO order);
        Task<Response> UpdateOrder(OrderDTO order);
        Task<Response> DeleteOrder(OrderDTO order);
        Task<IEnumerable<OrderDTO>> GetOrders();
        Task<OrderDTO> GetOrder(int id);
        Task<IEnumerable<OrderDetailsDTO>> GetOrdersByClientId(int clientId);
    }
}