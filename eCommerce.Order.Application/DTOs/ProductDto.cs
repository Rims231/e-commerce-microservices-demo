using System.ComponentModel.DataAnnotations;

namespace eCommerce.Order.Application.DTOs
{
    public record ProductDTO
    (
        int Id,

        [Required]
        string Name,

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        decimal Price,

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        int Quantity
    );
}