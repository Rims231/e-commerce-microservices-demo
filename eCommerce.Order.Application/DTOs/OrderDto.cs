using System.ComponentModel.DataAnnotations;

namespace eCommerce.Order.Application.DTOs
{
    public record OrderDTO
    (
        int Id,

        [Required]
        int ProductId,

        [Required]
        int ClientId,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Purchase quantity must be greater than 0")]
        int PurchaseQuantity,

        DateTime OrderedDate
    );
}