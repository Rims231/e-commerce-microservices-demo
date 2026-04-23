using eCommerce.Product.Domain.Interfaces;
using eCommerce.Product.Infrastructure.Data;
using eCommerce.SharedLibrary.Logs;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace eCommerce.Product.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<eCommerce.SharedLibrary.Response.Response> CreateAsync(Domain.Entities.Product entity)
        {
            try
            {
                var added = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                return added.Id > 0
                    ? new eCommerce.SharedLibrary.Response.Response(true, $"{entity.Name} created successfully")
                    : new eCommerce.SharedLibrary.Response.Response(false, "Error occurred creating product");
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new eCommerce.SharedLibrary.Response.Response(false, "Error occurred creating product");
            }
        }

        public async Task<eCommerce.SharedLibrary.Response.Response> UpdateAsync(Domain.Entities.Product entity)
        {
            try
            {
                var product = await context.Products.FindAsync(entity.Id);
                if (product is null)
                    return new eCommerce.SharedLibrary.Response.Response(false, "Product not found");

                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new eCommerce.SharedLibrary.Response.Response(true, $"{entity.Name} updated successfully");
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new eCommerce.SharedLibrary.Response.Response(false, "Error occurred updating product");
            }
        }

        public async Task<eCommerce.SharedLibrary.Response.Response> DeleteAsync(Domain.Entities.Product entity)
        {
            try
            {
                var product = await context.Products.FindAsync(entity.Id);
                if (product is null)
                    return new eCommerce.SharedLibrary.Response.Response(false, "Product not found");

                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new eCommerce.SharedLibrary.Response.Response(true, $"{entity.Name} deleted successfully");
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new eCommerce.SharedLibrary.Response.Response(false, "Error occurred deleting product");
            }
        }

        public async Task<IEnumerable<Domain.Entities.Product>> GetAllAsync()
        {
            try
            {
                return await context.Products.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                throw;
            }
        }

        public async Task<Domain.Entities.Product?> GetByIdAsync(int id)
        {
            try
            {
                return await context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                throw;
            }
        }

        public async Task<Domain.Entities.Product?> GetByAsync(Expression<Func<Domain.Entities.Product, bool>> predicate)
        {
            try
            {
                return await context.Products.Where(predicate).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                throw;
            }
        }
    }
}