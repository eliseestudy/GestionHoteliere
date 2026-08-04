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
    }
}
