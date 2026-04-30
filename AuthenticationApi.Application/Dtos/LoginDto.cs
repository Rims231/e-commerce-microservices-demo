using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.Dtos
{
    public record LoginDTO
    (
        [Required]
        [EmailAddress]
        string Email,

        [Required]
        string Password
    );
}