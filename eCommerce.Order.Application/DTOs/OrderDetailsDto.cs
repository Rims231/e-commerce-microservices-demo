using System.ComponentModel.DataAnnotations;

namespace eCommerce.Order.Application.DTOs
{
    public record OrderDetailsDTO
    (
        int OrderId,

        [Required]
        int ProductId,

        [Required]
        int ClientId,

        [Required]
        string ClientName,

        [Required]
        [EmailAddress]
        string Email,

        [Required]
        string Address,

        [Required]
        [Phone]
        string TelephoneNumber,

        [Required]
        string ProductName,

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        decimal ProductPrice,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        int PurchaseQuantity,

        DateTime OrderedDate
    );
}