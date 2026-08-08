using Application.Repositories;
using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(GestionHoteliereDbContext context)
            : base(context)
        {
        }
        public async Task<Client?> GetParIdentifiantNationalAsync(string identifiant)
        {
            return await Context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdentifiantNational == identifiant);
        }

        public async Task<Client?> GetParEmailAsync(string email)
        {
            return await Context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<IReadOnlyList<Client>> RechercherAsync(string terme)
        {
            if (string.IsNullOrWhiteSpace(terme)) return new List<Client>();
            terme = terme.ToLower();
            return await Context.Clients
                .Where(c => (c.Nom != null && c.Nom.ToLower().Contains(terme))
                    || (c.Prenom != null && c.Prenom.ToLower().Contains(terme))
                    || (c.Email != null && c.Email.ToLower().Contains(terme))
                    || (c.Telephone != null && c.Telephone.ToLower().Contains(terme)))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<(Client client, decimal totalDepense)>> GetTopClientsParDepensesAsync(int top = 10)
        {
            var totals = await Context.Factures
                .GroupBy(f => f.ClientId)
                .Select(g => new { ClientId = g.Key, Total = g.Sum(f => f.MontantTTC) })
                .OrderByDescending(x => x.Total)
                .Take(top)
                .ToListAsync();

            var result = new List<(Client client, decimal totalDepense)>();
            foreach (var t in totals)
            {
                var client = await Context.Clients.FindAsync(t.ClientId);
                if (client != null) result.Add((client, t.Total));
            }
            return result;
        }
    }
}