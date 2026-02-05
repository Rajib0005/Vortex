using System.ComponentModel.DataAnnotations;

namespace Vortex.Application.Dtos;

public class SetPasswordDto
{
    [Required]
    public required string Token { get; set; }

    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public required string NewPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    public required string ConfirmPassword { get; set; }
}
