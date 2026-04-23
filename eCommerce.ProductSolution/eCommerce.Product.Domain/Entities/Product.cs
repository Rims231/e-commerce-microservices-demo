using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce.Product.Domain.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        //public string Description { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        //[Required]
        //public string Category { get; set; } = string.Empty;

        public int Quantity { get; set; }

        //public string ImageUrl { get; set; } = string.Empty;

        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}