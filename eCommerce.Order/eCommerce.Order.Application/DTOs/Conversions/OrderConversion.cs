using eCommerce.Order.Application.DTOs;

namespace eCommerce.Order.Application.Conversions
{
    public static class OrderConversion
    {
        public static Domain.Entities.Order ToEntity(OrderDTO dto) => new()
        {
            Id = dto.Id,
            ProductId = dto.ProductId,
            ClientId = dto.ClientId,
            PurchaseQuantity = dto.PurchaseQuantity,
            OrderedDate = dto.OrderedDate
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(
            Domain.Entities.Order? order,
            IEnumerable<Domain.Entities.Order>? orders)
        {
            // Single order
            if (order is not null || orders is null)
            {
                var singleDto = new OrderDTO
                (
                    order!.Id,
                    order.ProductId,
                    order.ClientId,
                    order.PurchaseQuantity,
                    order.OrderedDate
                );
                return (singleDto, null);
            }

            // List of orders
            if (orders is not null || order is null)
            {
                var dtoList = orders!.Select(o => new OrderDTO
                (
                    o.Id,
                    o.ProductId,
                    o.ClientId,
                    o.PurchaseQuantity,
                    o.OrderedDate
                ));
                return (null, dtoList);
            }

            return (null, null);
        }
    }
}