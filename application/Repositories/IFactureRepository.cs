using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionHoteliere.Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IFactureRepository : IRepository<Facture>
    {
        /// <summary>
        /// Génère une facture à partir d'un séjour (ne sauvegarde pas automatiquement).
        /// </summary>
        Task<Facture> GenererPourSejourAsync(int sejourId);

        /// <summary>
        /// Calcule le montant total (TTC) attendu pour un séjour.
        /// </summary>
        Task<decimal> CalculerMontantFactureAsync(int sejourId);

        /// <summary>
        /// Récupère les factures dont le montant payé est inférieur au montant total.
        /// </summary>
        Task<IReadOnlyList<Facture>> GetNonRegleesAsync();

        /// <summary>
        /// Récupère les factures en retard par rapport à une date de référence.
        /// </summary>
        Task<IReadOnlyList<Facture>> GetEnRetardAsync(DateTimeOffset referenceDate);

        /// <summary>
        /// Met à jour le statut d'une facture.
        /// </summary>
        Task SetStatutAsync(int factureId, FactureStatut statut);
    }
}
