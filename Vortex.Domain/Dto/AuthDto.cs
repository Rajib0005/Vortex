using System.ComponentModel.DataAnnotations;

namespace Vortex.Domain.Dto;

public class AuthDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}