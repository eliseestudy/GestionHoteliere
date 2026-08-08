using Application.Repositories;
using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SejourRepository : Repository<Sejour>, ISejourRepository
    {
        public SejourRepository(GestionHoteliereDbContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyList<Sejour>> GetEnCoursAsync()
        {
            return await Context.Sejours
                .Where(s => s.Statut == SejourStatut.EnCours)
                .Include(s => s.Chambre)
                .Include(s => s.Client)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Sejour>> GetHistoriquesParClientAsync(int clientId)
        {
            return await Context.Sejours
                .Where(s => s.ClientId == clientId && s.Statut != SejourStatut.EnCours)
                .Include(s => s.Chambre)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Sejour> DemarrerSejourDepuisReservationAsync(int reservationId, int chambreId, int clientId)
        {
            var reservation = await Context.Reservations
                .Include(r => r.TypeChambre)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            var sejour = new Sejour
            {
                ReservationId = reservationId,
                ClientId = clientId,
                ChambreId = chambreId,
                DateEntree = reservation?.DateArrivee ?? DateTimeOffset.UtcNow,
                NbNuits = reservation is not null ? (int)(reservation.DateDepart - reservation.DateArrivee).TotalDays : 1,
                TarifApplique = reservation?.TypeChambre?.PrixParNuit ?? 0m,
                Statut = SejourStatut.EnCours
            };

            await DbSet.AddAsync(sejour);
            return sejour;
        }

        public async Task CloturerSejourAsync(int sejourId, DateTimeOffset dateSortie, decimal montantApplique, decimal remise)
        {
            var sejour = await Context.Sejours.FindAsync(sejourId);
            if (sejour is null) return;
            sejour.DateSortie = dateSortie;
            sejour.TarifApplique = montantApplique;
            sejour.Remise = remise;
            sejour.Statut = SejourStatut.Termine;
            Update(sejour);
        }
    }
}
