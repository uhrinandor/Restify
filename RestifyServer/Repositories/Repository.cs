using Microsoft.EntityFrameworkCore;
using RestifyServer.Interfaces;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class Repository<T> : IRepository<T> where T : Entity, new()
{
    private readonly RestifyContext _db;
    private readonly DbSet<T> _set;

    public Repository(RestifyContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    public IQueryable<T> Query(bool asNoTracking = true) => asNoTracking ? _set.AsNoTracking() : _set;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default, bool asNoTracking = true)
    {
        if (!asNoTracking) return await _set.FindAsync(new object[] { id }, ct);

        return await _set.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) => _set.AsNoTracking().AnyAsync(x => x.Id == id, ct);

    public void Add(T entity) => _set.Add(entity);
    public void AddRange(IEnumerable<T> entities) => _set.AddRange(entities);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public void RemoveById(Guid id)
    {
        var strub = new T { Id = id };
        _db.Entry(strub).State = EntityState.Deleted;
    }

    public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
