using System.ComponentModel.DataAnnotations;

namespace Api.Contracts;

public sealed class TypeChambreRequest
{
    [Required]
    [MaxLength(100)]
    public string Libelle { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(1, 100)]
    public int Capacite { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal PrixParNuit { get; set; }
}

public sealed record TypeChambreResponse(
    int Id,
    string Libelle,
    string? Description,
    int Capacite,
    decimal PrixParNuit);