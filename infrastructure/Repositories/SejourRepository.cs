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
    }
}
