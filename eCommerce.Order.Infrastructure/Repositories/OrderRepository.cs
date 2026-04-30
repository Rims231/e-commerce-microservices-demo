using eCommerce.Order.Application.Interfaces;
using eCommerce.Order.Infrastructure.Data;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using OrderEntity = eCommerce.Order.Domain.Entities.Order;

namespace eCommerce.Order.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(OrderEntity entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0
                    ? new Response(true, "Order placed successfully")
                    : new Response(false, "Error occurred while placing order");
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new Response(false, ex.InnerException?.Message ?? ex.Message);
            }
        }

        public async Task<Response> UpdateAsync(OrderEntity entity)
        {
            try
            {
                var order = await context.Orders.FindAsync(entity.Id);
                if (order is null)
                    return new Response(false, "Order not found");

                context.Entry(order).State = EntityState.Detached;
                context.Orders.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, "Order updated successfully");
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new Response(false, "Error occurred while updating order");
            }
        }

        public async Task<Response> DeleteAsync(OrderEntity entity)
        {
            try
            {
                var order = await context.Orders.FindAsync(entity.Id);
                if (order is null)
                    return new Response(false, "Order not found");

                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                return new Response(true, "Order deleted successfully");
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new Response(false, "Error occurred while deleting order");
            }
        }

        public async Task<IEnumerable<OrderEntity>> GetAllAsync()
        {
            try
            {
                return await context.Orders.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return Enumerable.Empty<OrderEntity>();
            }
        }

        public async Task<OrderEntity?> GetByIdAsync(int id)
        {
            try
            {
                return await context.Orders.FindAsync(id);
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return null;
            }
        }

        public async Task<OrderEntity?> GetByAsync(Expression<Func<OrderEntity, bool>> predicate)
        {
            try
            {
                return await context.Orders.Where(predicate).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return null;
            }
        }

        public async Task<IEnumerable<OrderEntity>> GetOrdersAsync(Expression<Func<OrderEntity, bool>> predicate)
        {
            try
            {
                return await context.Orders.Where(predicate).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return Enumerable.Empty<OrderEntity>();
            }
        }
    }
}