using System.ComponentModel.DataAnnotations;

namespace Shared.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required]
    [MinLength(32, ErrorMessage = "SecretKey must be at least 32 characters.")]
    public string SecretKey { get; set; } = string.Empty;

    [Range(1, 10080)]
    public int ExpirationMinutes { get; set; } = 60;
}
