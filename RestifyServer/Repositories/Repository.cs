using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestifyServer.Interfaces;
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

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default, bool asNoTracking = true)
    {
        if (!asNoTracking) return await _set.FindAsync(new object[] { id }, ct);

        return await _set.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => _set.AsNoTracking().AnyAsync(x => x.Id == id, ct);

    public virtual Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default,
        bool asNoTracking = true)
    {
        var q = asNoTracking ? _set.AsNoTracking() : _set;

        return q.FirstOrDefaultAsync(predicate, ct);
    }

    public virtual Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default,
        bool asNoTracking = true)
    {
        var q = asNoTracking ? _set.AsNoTracking() : _set;

        return q.Where(predicate).ToListAsync(ct);
    }

    public virtual Task<List<T>> ListAsync(CancellationToken ct = default,
        bool asNoTracking = true)
    {
        var q = asNoTracking ? _set.AsNoTracking() : _set;

        return q.ToListAsync(ct);
    }

    public void Add(T entity) => _set.Add(entity);
    public void AddRange(IEnumerable<T> entities) => _set.AddRange(entities);

    public void Remove(T entity) => _set.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);
}
