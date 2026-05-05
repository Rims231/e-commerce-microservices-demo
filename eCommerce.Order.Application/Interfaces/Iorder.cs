using eCommerce.SharedLibrary.Interface;
using System.Linq.Expressions;
using OrderEntity = eCommerce.Order.Domain.Entities.Order;

namespace eCommerce.Order.Application.Interfaces
{
    public interface IOrder : IGenericInterface<OrderEntity>
    {
        Task<IEnumerable<OrderEntity>> GetOrdersAsync(Expression<Func<OrderEntity, bool>> predicate);
    }
}