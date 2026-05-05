using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Domain.Entities
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string TelephoneNumber { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;


        [Required]
        public DateTime DateRegister { get; set; } = DateTime.Now;
    }
}