using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class CategoryRepository(RestifyContext db) : Repository<Category>(db)
{
    public override Task<Category?> FirstOrDefaultAsync(Expression<Func<Category, bool>> predicate,
        CancellationToken ct = default,
        bool asNoTracking = true)
    {
        var q = asNoTracking ? _set.AsNoTracking() : _set;

        return q.Include(x => x.Parent).Include(x => x.Children).FirstOrDefaultAsync(predicate, ct);
    }

    public override Task<List<Category>> ListAsync(Expression<Func<Category, bool>> predicate, CancellationToken ct = default,
        bool asNoTracking = true)
    {
        var q = asNoTracking ? _set.AsNoTracking() : _set;

        return q.Include(x => x.Parent).Include(x => x.Children).Where(predicate).ToListAsync(ct);
    }

    public override Task<List<Category>> ListAsync(CancellationToken ct = default,
        bool asNoTracking = true)
    {
        var q = asNoTracking ? _set.AsNoTracking() : _set;

        return q.Include(x => x.Parent).Include(x => x.Children).ToListAsync(ct);
    }
}
