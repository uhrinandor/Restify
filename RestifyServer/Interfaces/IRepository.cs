using RestifyServer.Models;

namespace RestifyServer.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    IQueryable<TEntity> Query(bool asNoTracking = true);

    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default, bool asNoTracking = true);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);

    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void Remove(TEntity entity);
    void RemoveById(Guid id);
    void RemoveRange(IEnumerable<TEntity> entities);

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
