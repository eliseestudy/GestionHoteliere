using Application.Repositories;
using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PaiementRepository : Repository<Paiement>, IPaiementRepository
    {
        public PaiementRepository(GestionHoteliereDbContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyList<Paiement>> GetParFactureAsync(int factureId)
        {
            return await Context.Paiements
                .Where(p => p.FactureId == factureId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Paiement> EnregistrerPaiementAsync(Paiement paiement)
        {
            await DbSet.AddAsync(paiement);
            return paiement;
        }

        public async Task ConfirmPaiementAsync(int paiementId, string? transactionRef = null)
        {
            var p = await Context.Paiements.FindAsync(paiementId);
            if (p is null) return;
            p.Statut = PaiementStatut.Effectue;
            if (!string.IsNullOrEmpty(transactionRef)) p.TransactionReference = transactionRef;
            Update(p);
        }

        public async Task<IReadOnlyList<Paiement>> GetEnAttenteAsync()
        {
            return await Context.Paiements
                .Where(p => p.Statut == PaiementStatut.Initie || p.Statut == PaiementStatut.Authorise)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Paiement?> GetParReferenceTransactionAsync(string transactionReference)
        {
            return await Context.Paiements
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.TransactionReference == transactionReference);
        }
    }
}
