using GestionHoteliere.Domain.Entities;

namespace Application.Repositories
{
    public interface ISejourRepository : IRepository<Sejour>
    {

        /// <summary>
        /// Récupère les séjours actuellement en cours.
        /// </summary>
        Task<IReadOnlyList<Sejour>> GetEnCoursAsync();

        /// <summary>
        /// Récupère l'historique des séjours d'un client (séjours terminés ou annulés).
        /// </summary>
        Task<IReadOnlyList<Sejour>> GetHistoriquesParClientAsync(int clientId);

        /// <summary>
        /// Démarre un séjour à partir d'une réservation (créé l'entité Sejour).
        /// Note: la coordination transactionnelle doit être gérée par un service.
        /// </summary>
        Task<Sejour> DemarrerSejourDepuisReservationAsync(int reservationId, int chambreId, int clientId);

        /// <summary>
        /// Clôture un séjour (date de sortie, montants, mise à jour de statut).
        /// </summary>
        Task CloturerSejourAsync(int sejourId, DateTimeOffset dateSortie, decimal montantApplique, decimal remise);

    }
}
