using Application.Repositories;
using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FactureRepository : Repository<Facture>, IFactureRepository
    {
        public FactureRepository(GestionHoteliereDbContext context)
            : base(context)
        {
        }

        public async Task<Facture> GenererPourSejourAsync(int sejourId)
        {
            var sejour = await Context.Sejours
                .Include(s => s.Client)
                .Include(s => s.Chambre)
                .FirstOrDefaultAsync(s => s.Id == sejourId);

            if (sejour is null) throw new InvalidOperationException("Séjour introuvable");

            var montantHt = sejour.NbNuits * sejour.TarifApplique - sejour.Remise;
            var taxe = 0m;
            var montantTtc = montantHt + taxe;

            var facture = new Facture
            {
                SejourId = sejourId,
                ClientId = sejour.ClientId,
                DateEmission = DateTimeOffset.UtcNow,
                MontantHT = montantHt,
                Taxe = taxe,
                MontantTTC = montantTtc,
                MontantPaye = 0m,
                Statut = FactureStatut.Emise
            };

            await DbSet.AddAsync(facture);
            return facture;
        }

        public async Task<decimal> CalculerMontantFactureAsync(int sejourId)
        {
            var sejour = await Context.Sejours.FindAsync(sejourId);
            if (sejour is null) return 0m;
            var montantHt = sejour.NbNuits * sejour.TarifApplique - sejour.Remise;
            var taxe = 0m;
            return montantHt + taxe;
        }

        public async Task<IReadOnlyList<Facture>> GetNonRegleesAsync()
        {
            return await Context.Factures
                .Where(f => f.MontantPaye < f.MontantTTC && f.Statut != FactureStatut.Annulee)
                .Include(f => f.Client)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Facture>> GetEnRetardAsync(DateTimeOffset referenceDate)
        {
            return await Context.Factures
                .Where(f => f.DateEcheance.HasValue && f.DateEcheance < referenceDate && f.MontantPaye < f.MontantTTC)
                .Include(f => f.Client)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SetStatutAsync(int factureId, FactureStatut statut)
        {
            var facture = await Context.Factures.FindAsync(factureId);
            if (facture is null) return;
            facture.Statut = statut;
            Update(facture);
        }
    }
}
