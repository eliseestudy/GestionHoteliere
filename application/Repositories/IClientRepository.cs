using GestionHoteliere.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Application.Repositories
{
    public interface IClientRepository: IRepository<Client>
    {
        /// <summary>
        /// Récupère un client par son identifiant national.
        /// </summary>
        Task<Client?> GetParIdentifiantNationalAsync(string identifiant);

        /// <summary>
        /// Récupère un client par son adresse e-mail.
        /// </summary>
        Task<Client?> GetParEmailAsync(string email);

        /// <summary>
        /// Recherche des clients par terme (nom, prénom, email, téléphone).
        /// </summary>
        Task<IReadOnlyList<Client>> RechercherAsync(string terme);

        /// <summary>
        /// Retourne les clients triés par dépenses totales (TTC) décroissantes.
        /// </summary>
        Task<IReadOnlyList<(Client client, decimal totalDepense)>> GetTopClientsParDepensesAsync(int top = 10);

    }
}
