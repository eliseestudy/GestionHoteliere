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
    }
}
