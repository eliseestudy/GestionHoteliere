using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestionHoteliere.Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Récupère un utilisateur par son nom d'utilisateur.
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Récupère un utilisateur par son adresse e-mail.
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Met à jour la date/heure de dernière connexion d'un utilisateur.
        /// </summary>
        Task SetDerniereConnexionAsync(int userId, DateTimeOffset when);

        /// <summary>
        /// Active ou désactive un compte utilisateur.
        /// </summary>
        Task SetActivationAsync(int userId, bool isActive);

        /// <summary>
        /// Récupère les utilisateurs d'un rôle donné.
        /// </summary>
        Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role);
    }
}
