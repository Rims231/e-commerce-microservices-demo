using eCommerce.Product.Application.DTOs;

namespace eCommerce.Product.Application.Conversions
{
    public static class ProductConversions
    {
        public static eCommerce.Product.Domain.Entities.Product ToEntity(ProductDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price,
            Quantity = dto.Quantity
        };

        public static ProductDto FromEntity(eCommerce.Product.Domain.Entities.Product entity) => new(
            entity.Id,
            entity.Name,
            entity.Price,
            entity.Quantity
        );

        public static IEnumerable<ProductDto> FromEntityList(IEnumerable<eCommerce.Product.Domain.Entities.Product> entities)
            => entities.Select(FromEntity).ToList();
    }
}