using System.ComponentModel.DataAnnotations;

namespace eCommerce.Product.Application.DTOs
{
    public record ProductDto
 (
        int Id,

        [Required]
        string Name,

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        decimal Price,

        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        int Quantity
    );
}
