using System.ComponentModel.DataAnnotations;

namespace SampleApi.DTOs;

public class SignUpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = default!;

    [Required]
    [MaxLength(256)]
    public string FullName { get; set; } = default!;
}
