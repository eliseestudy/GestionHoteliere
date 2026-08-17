using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Api.Contracts;

public sealed class ReservationRequest
{
    [Required]
    [MaxLength(50)]
    public string NumeroReservation { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ClientId { get; set; }

    public int? ChambreId { get; set; }

    public int? TypeChambreId { get; set; }

    public DateTimeOffset DateArrivee { get; set; }

    public DateTimeOffset DateDepart { get; set; }

    public ReservationStatut Statut { get; set; } = ReservationStatut.EnAttente;

    [Range(1, 100)]
    public int NombrePersonnes { get; set; } = 1;

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal MontantEstime { get; set; }
}

public sealed record ReservationResponse(
    int Id,
    string NumeroReservation,
    int ClientId,
    int? ChambreId,
    int? TypeChambreId,
    DateTimeOffset DateCreation,
    DateTimeOffset DateArrivee,
    DateTimeOffset DateDepart,
    ReservationStatut Statut,
    int NombrePersonnes,
    decimal MontantEstime);
