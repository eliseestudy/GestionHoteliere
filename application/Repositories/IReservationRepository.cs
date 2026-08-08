using GestionHoteliere.Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IReservationRepository : IRepository<Reservation>
    {
        /// <summary>
        /// Récupère une réservation par son numéro.
        /// </summary>
        Task<Reservation?> GetByNumeroAsync(string numeroReservation);

        /// <summary>
        /// Récupère les réservations associées à un client.
        /// </summary>
        Task<IReadOnlyList<Reservation>> GetParClientAsync(int clientId);

        /// <summary>
        /// Récupère les réservations qui chevauchent une période donnée.
        /// </summary>
        Task<IReadOnlyList<Reservation>> GetParPeriodeAsync(DateTimeOffset start, DateTimeOffset end);

        /// <summary>
        /// Vérifie s'il existe un chevauchement de réservation pour une chambre et une période données.
        /// </summary>
        Task<bool> ExisteChevauchementAsync(int? chambreId, DateTimeOffset dateArrivee, DateTimeOffset dateDepart);

        /// <summary>
        /// Modifie le statut d'une réservation.
        /// </summary>
        Task SetStatutAsync(int reservationId, ReservationStatut statut);

    }
}
