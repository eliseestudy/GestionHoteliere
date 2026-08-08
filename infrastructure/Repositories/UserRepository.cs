using Application.Repositories;
using Domain.Enums;
using GestionHoteliere.Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(GestionHoteliereDbContext context)
            : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await Context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task SetDerniereConnexionAsync(int userId, DateTimeOffset when)
        {
            var u = await Context.Users.FindAsync(userId);
            if (u is null) return;
            u.DerniereConnexion = when;
            Update(u);
        }

        public async Task SetActivationAsync(int userId, bool isActive)
        {
            var u = await Context.Users.FindAsync(userId);
            if (u is null) return;
            u.IsActive = isActive;
            Update(u);
        }

        public async Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role)
        {
            return await Context.Users
                .Where(u => u.Role == role)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
