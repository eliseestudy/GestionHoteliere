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
    }
}
