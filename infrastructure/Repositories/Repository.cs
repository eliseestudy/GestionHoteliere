using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly GestionHoteliereDbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(GestionHoteliereDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync() =>
            await DbSet.AsNoTracking().ToListAsync();
        public async Task<T?> GetByIdAsync(int id) =>
            await DbSet.FindAsync(id);
        public async Task AddAsync(T entity) => await DbSet.AddAsync(entity);
        public void Update(T entity) => DbSet.Update(entity);
        public void Delete(T entity) => DbSet.Remove(entity);
    }
}

