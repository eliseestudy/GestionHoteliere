using System.ComponentModel.DataAnnotations;

namespace Api.Contracts;

public sealed class ClientRequest
{
    [Required]
    [MaxLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [Phone]
    [MaxLength(50)]
    public string? Telephone { get; set; }

    [MaxLength(300)]
    public string? Adresse { get; set; }

    public DateTime? DateNaissance { get; set; }

    [MaxLength(100)]
    public string? IdentifiantNational { get; set; }
}

public sealed record ClientResponse(
    int Id,
    string Prenom,
    string Nom,
    string? Email,
    string? Telephone,
    string? Adresse,
    DateTime? DateNaissance,
    string? IdentifiantNational);
