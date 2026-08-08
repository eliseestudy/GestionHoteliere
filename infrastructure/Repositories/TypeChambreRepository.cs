using Application.Repositories;
using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TypeChambreRepository : Repository<TypeChambre>, ITypeChambreRepository
    {
        public TypeChambreRepository(GestionHoteliereDbContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyList<TypeChambre>> GetDisponiblesAsync(DateTime dateArrivee, DateTime dateDepart)
        {
            return await Context.TypesChambres
                .Where(t => t.Chambres.Any(c => c.Statut != ChambreStatut.Maintenance
                    && !c.Reservations.Any(r => r.Statut != ReservationStatut.Annulee
                        && dateArrivee < r.DateDepart
                        && dateDepart > r.DateArrivee)))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<TypeChambre>> GetParCapaciteEtPrixAsync(int capaciteMin, decimal prixMax)
        {
            return await Context.TypesChambres
                .Where(t => t.Capacite >= capaciteMin && t.PrixParNuit <= prixMax)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
