using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Api.Contracts;

public sealed class SejourRequest
{
    public int? ReservationId { get; set; }

    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }

    [Range(1, int.MaxValue)]
    public int ChambreId { get; set; }

    public DateTimeOffset DateEntree { get; set; }

    public DateTimeOffset? DateSortie { get; set; }

    [Range(1, 3650)]
    public int NbNuits { get; set; } = 1;

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal TarifApplique { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal Remise { get; set; }

    public SejourStatut Statut { get; set; } = SejourStatut.EnCours;
}

public sealed record SejourResponse(
    int Id,
    int? ReservationId,
    int ClientId,
    int ChambreId,
    DateTimeOffset DateEntree,
    DateTimeOffset? DateSortie,
    int NbNuits,
    decimal TarifApplique,
    decimal Remise,
    SejourStatut Statut);
