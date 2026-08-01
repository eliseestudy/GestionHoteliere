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

    }

}