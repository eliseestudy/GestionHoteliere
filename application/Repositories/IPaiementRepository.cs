using GestionHoteliere.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IPaiementRepository : IRepository<Paiement>
    {
        /// <summary>
        /// Récupère la liste des paiements pour une facture donnée.
        /// </summary>
        Task<IReadOnlyList<Paiement>> GetParFactureAsync(int factureId);

        /// <summary>
        /// Enregistre un paiement (ajoute l'entité au contexte).
        /// </summary>
        Task<Paiement> EnregistrerPaiementAsync(Paiement paiement);

        /// <summary>
        /// Confirme un paiement (met à jour le statut et référence transaction si fournie).
        /// </summary>
        Task ConfirmPaiementAsync(int paiementId, string? transactionRef = null);

        /// <summary>
        /// Récupère les paiements en attente ou autorisés.
        /// </summary>
        Task<IReadOnlyList<Paiement>> GetEnAttenteAsync();

        /// <summary>
        /// Récupère un paiement par sa référence de transaction.
        /// </summary>
        Task<Paiement?> GetParReferenceTransactionAsync(string transactionReference);
    }
}
