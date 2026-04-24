using System.ComponentModel.DataAnnotations;

namespace eCommerce.Order.Domain.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int PurchaseQuantity { get; set; }

        public DateTime OrderedDate { get; set; } = DateTime.UtcNow;
    }
}