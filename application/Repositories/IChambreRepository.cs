using GestionHoteliere.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IChambreRepository : IRepository<Chambre>
    {

        Task<IReadOnlyList<Chambre>> GetChambresDisponiblesAsync(
            DateTime dateArrivee, DateTime dateDepart);

        Task<Chambre?> GetAvecTypeAsync(int id);

        /// <summary>
        /// Récupère une chambre par son numéro unique.
        /// </summary>
        Task<Chambre?> GetByNumeroAsync(string numero);

        /// <summary>
        /// Compte le nombre de chambres disponibles d'un type donné pour une période.
        /// </summary>
        Task<int> CountDisponiblesParTypeAsync(int typeChambreId, DateTime dateArrivee, DateTime dateDepart);

        /// <summary>
        /// Récupère la liste des chambres disponibles d'un type pour une période.
        /// </summary>
        Task<IReadOnlyList<Chambre>> GetDisponiblesParTypeAsync(int typeChambreId, DateTime dateArrivee, DateTime dateDepart);

        /// <summary>
        /// Récupère une chambre avec ses relations (type, réservations, séjours).
        /// </summary>
        Task<Chambre?> GetAvecDetailsAsync(int id);

        /// <summary>
        /// Marque une chambre comme étant en maintenance.
        /// </summary>
        Task MarkAsMaintenanceAsync(int chambreId);

    }

}