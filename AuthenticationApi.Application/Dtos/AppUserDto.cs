using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record AppUserDTO
    (
        int Id,

        [Required]
        string Name,

        [Required]
        [Phone]
        string TelephoneNumber,

        [Required]
        string Address,

        [Required]
        [EmailAddress]
        string Email,

        [Required]
        string Password,

        [Required]
        string Role
    );
}