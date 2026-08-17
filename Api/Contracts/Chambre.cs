using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Api.Contracts;

public sealed class ChambreRequest
{
    [Required]
    [MaxLength(50)]
    public string Numero { get; set; } = string.Empty;

    public int Etage { get; set; }

    [Range(1, int.MaxValue)]
    public int TypeChambreId { get; set; }

    public ChambreStatut Statut { get; set; } = ChambreStatut.Libre;

    [Range(1, 100)]
    public int NbLits { get; set; } = 1;

    [MaxLength(500)]
    public string? Observations { get; set; }
}

public sealed record ChambreResponse(
    int Id,
    string Numero,
    int Etage,
    int TypeChambreId,
    ChambreStatut Statut,
    int NbLits,
    string? Observations);
