using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public abstract class Repository<T> : IRepository<T> where T : Entity, new()
{
    private readonly RestifyContext _db;
    protected readonly DbSet<T> _set;

    public Repository(RestifyContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    protected virtual IQueryable<T> ListQueryable(bool asNoTracking = true) => asNoTracking ? _set.AsNoTracking() : _set;
    protected virtual IQueryable<T> SingleQueryable(bool asNoTracking = true) => asNoTracking ? _set.AsNoTracking() : _set;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default, bool asNoTracking = true)
    {
        var q = SingleQueryable(asNoTracking);

        return await q.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => _set.AsNoTracking().AnyAsync(x => x.Id == id, ct);

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default,
        bool asNoTracking = true)
    {
        var q = SingleQueryable(asNoTracking);

        return q.FirstOrDefaultAsync(predicate, ct);
    }

    public Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default,
        bool asNoTracking = true)
    {
        var q = ListQueryable(asNoTracking);

        return q.Where(predicate).ToListAsync(ct);
    }

    public Task<List<T>> ListAsync(CancellationToken ct = default,
        bool asNoTracking = true)
    {
        var q = ListQueryable(asNoTracking);

        return q.ToListAsync(ct);
    }

    public void Add(T entity) => _set.Add(entity);
    public void AddRange(IEnumerable<T> entities) => _set.AddRange(entities);

    public void Remove(T entity) => _set.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);
}
