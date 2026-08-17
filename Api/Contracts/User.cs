using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Api.Contracts;

public sealed class UserRequest
{
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [Required]
    [MaxLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Receptionniste;

    [MaxLength(100)]
    public string? Prenom { get; set; }

    [MaxLength(100)]
    public string? Nom { get; set; }

    public bool IsActive { get; set; } = true;
}

public sealed record UserResponse(
    int Id,
    string Username,
    string? Email,
    UserRole Role,
    string? Prenom,
    string? Nom,
    bool IsActive,
    DateTimeOffset? DerniereConnexion);
