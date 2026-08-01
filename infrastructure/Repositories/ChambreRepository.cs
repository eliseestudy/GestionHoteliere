using Application.Repositories;
using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ChambreRepository : Repository<Chambre>, IChambreRepository
    {
        public ChambreRepository(GestionHoteliereDbContext context)
            : base(context)
        {
        }

        // Regle de chevauchement :
        // deux sejours se chevauchent si
        // arriveeDemandee < departExistant ET departDemande > arriveeExistante.
        public async Task<IReadOnlyList<Chambre>> GetChambresDisponiblesAsync(
            DateTime dateArrivee, DateTime dateDepart)
        {
            return await Context.Chambres
                .Include(x => x.TypeChambre)
                .Where(x => x.Statut != ChambreStatut.Maintenance
                    && !x.Reservations.Any(r =>
                        r.Statut != ReservationStatut.Annulee
                        && dateArrivee < r.DateDepart
                        && dateDepart > r.DateArrivee))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Chambre?> GetAvecTypeAsync(int id)
        {
            return await Context.Chambres
                .Include(x => x.TypeChambre)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}