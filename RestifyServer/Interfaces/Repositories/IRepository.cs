using System.Linq.Expressions;
using RestifyServer.Models;

namespace RestifyServer.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default, bool asNoTracking = true);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default, bool asNoTracking = true);
    Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default, bool asNoTracking = true);
    Task<List<TEntity>> ListAsync(CancellationToken ct = default, bool asNoTracking = true);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveRange(IEnumerable<TEntity> entities);
}
