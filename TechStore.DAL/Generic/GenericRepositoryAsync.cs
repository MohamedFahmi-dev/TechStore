using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechStore.DAL.Context;
using TechStore.Domain.Common;

namespace TechStore.DAL.Generic;

public class GenericRepositoryAsync<TEntity> : IGenericRepositoryAsync<TEntity> where TEntity : BaseEntity
{
    protected readonly TechDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepositoryAsync(TechDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        var entity = await DbSet.FindAsync(id);
        if (entity is null)
        {
            return null;
        }

        return entity;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await DbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await DbSet.Where(predicate).ToListAsync();
    }

    public IQueryable<TEntity> GetTableNoTracking()
    {
        return DbSet.AsNoTracking().AsQueryable();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
    {
        if (predicate == null)
            return await DbSet.CountAsync();
            
        return await DbSet.CountAsync(predicate);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await DbSet.AddRangeAsync(entities);
        return entities;
    }

    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        DbSet.RemoveRange(entities);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }
}

