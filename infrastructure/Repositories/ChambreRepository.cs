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

        public async Task<Chambre?> GetByNumeroAsync(string numero)
        {
            return await Context.Chambres
                .Include(x => x.TypeChambre)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Numero == numero);
        }

        public async Task<int> CountDisponiblesParTypeAsync(int typeChambreId, DateTime dateArrivee, DateTime dateDepart)
        {
            return await Context.Chambres
                .Where(x => x.TypeChambreId == typeChambreId
                    && x.Statut != ChambreStatut.Maintenance
                    && !x.Reservations.Any(r => r.Statut != ReservationStatut.Annulee
                        && dateArrivee < r.DateDepart
                        && dateDepart > r.DateArrivee))
                .CountAsync();
        }

        public async Task<IReadOnlyList<Chambre>> GetDisponiblesParTypeAsync(int typeChambreId, DateTime dateArrivee, DateTime dateDepart)
        {
            return await Context.Chambres
                .Include(x => x.TypeChambre)
                .Where(x => x.TypeChambreId == typeChambreId
                    && x.Statut != ChambreStatut.Maintenance
                    && !x.Reservations.Any(r => r.Statut != ReservationStatut.Annulee
                        && dateArrivee < r.DateDepart
                        && dateDepart > r.DateArrivee))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Chambre?> GetAvecDetailsAsync(int id)
        {
            return await Context.Chambres
                .Include(x => x.TypeChambre)
                .Include(x => x.Reservations)
                .Include(x => x.Sejours)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task MarkAsMaintenanceAsync(int chambreId)
        {
            var chambre = await Context.Chambres.FindAsync(chambreId);
            if (chambre is null) return;
            chambre.Statut = ChambreStatut.Maintenance;
            Update(chambre);
        }
    }
}