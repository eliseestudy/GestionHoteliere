using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Api.Contracts;

public sealed class FactureRequest
{
    [Required]
    [MaxLength(50)]
    public string NumeroFacture { get; set; } = string.Empty;

    public int? SejourId { get; set; }

    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }

    public DateTimeOffset DateEmission { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? DateEcheance { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal MontantHT { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal Taxe { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal MontantTTC { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal MontantPaye { get; set; }

    public FactureStatut Statut { get; set; } = FactureStatut.Brouillon;
}

public sealed record FactureResponse(
    int Id,
    string NumeroFacture,
    int? SejourId,
    int ClientId,
    DateTimeOffset DateEmission,
    DateTimeOffset? DateEcheance,
    decimal MontantHT,
    decimal Taxe,
    decimal MontantTTC,
    decimal MontantPaye,
    FactureStatut Statut);
