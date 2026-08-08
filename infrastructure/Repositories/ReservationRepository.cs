using Application.Repositories;
using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(GestionHoteliereDbContext context)
            : base(context)
        {
        }

        public async Task<Reservation?> GetByNumeroAsync(string numeroReservation)
        {
            return await Context.Reservations
                .Include(r => r.Client)
                .Include(r => r.Chambre)
                .Include(r => r.TypeChambre)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.NumeroReservation == numeroReservation);
        }

        public async Task<IReadOnlyList<Reservation>> GetParClientAsync(int clientId)
        {
            return await Context.Reservations
                .Where(r => r.ClientId == clientId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Reservation>> GetParPeriodeAsync(DateTimeOffset start, DateTimeOffset end)
        {
            return await Context.Reservations
                .Where(r => r.DateArrivee < end && r.DateDepart > start)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExisteChevauchementAsync(int? chambreId, DateTimeOffset dateArrivee, DateTimeOffset dateDepart)
        {
            if (!chambreId.HasValue) return false;
            return await Context.Reservations
                .AnyAsync(r => r.ChambreId == chambreId
                    && r.Statut != ReservationStatut.Annulee
                    && dateArrivee < r.DateDepart
                    && dateDepart > r.DateArrivee);
        }

        public async Task SetStatutAsync(int reservationId, ReservationStatut statut)
        {
            var res = await Context.Reservations.FindAsync(reservationId);
            if (res is null) return;
            res.Statut = statut;
            Update(res);
        }
    }
}
