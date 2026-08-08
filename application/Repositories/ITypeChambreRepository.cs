using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionHoteliere.Domain.Entities;

namespace Application.Repositories
{
    public interface ITypeChambreRepository : IRepository<TypeChambre>
    {
        /// <summary>
        /// Récupère les types de chambre disposant d'au moins une chambre disponible sur la période.
        /// </summary>
        Task<IReadOnlyList<TypeChambre>> GetDisponiblesAsync(DateTime dateArrivee, DateTime dateDepart);

        /// <summary>
        /// Recherche les types de chambre par capacité minimale et prix maximal par nuit.
        /// </summary>
        Task<IReadOnlyList<TypeChambre>> GetParCapaciteEtPrixAsync(int capaciteMin, decimal prixMax);
    }
}
