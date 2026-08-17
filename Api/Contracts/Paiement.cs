using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Api.Contracts;

public sealed class PaiementRequest
{
    [Range(1, int.MaxValue)]
    public int FactureId { get; set; }

    public DateTimeOffset DatePaiement { get; set; } = DateTimeOffset.UtcNow;

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal Montant { get; set; }

    public ModePaiement Mode { get; set; } = ModePaiement.Carte;

    public PaiementStatut Statut { get; set; } = PaiementStatut.Initie;

    [MaxLength(200)]
    public string? TransactionReference { get; set; }

    public int? UserId { get; set; }
}

public sealed record PaiementResponse(
    int Id,
    int FactureId,
    DateTimeOffset DatePaiement,
    decimal Montant,
    ModePaiement Mode,
    PaiementStatut Statut,
    string? TransactionReference,
    int? UserId);
