using System.Linq;
using System.Linq.Expressions;
using TechStore.Domain.Common;

namespace TechStore.DAL.Generic;

public interface IGenericRepositoryAsync<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> GetTableNoTracking();
    Task<TEntity> AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);
    void RemoveRange(IEnumerable<TEntity> entities);
    Task<int> SaveChangesAsync();
}

