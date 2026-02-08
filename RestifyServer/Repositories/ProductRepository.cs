using Microsoft.EntityFrameworkCore;
using RestifyServer.Data;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class ProductRepository(RestifyContext db) : Repository<Product>(db)
{
    protected override IQueryable<Product> ListQueryable(bool asNoTracking = true)
        => base.ListQueryable(asNoTracking).Include(x => x.Category);

    protected override IQueryable<Product> SingleQueryable(bool asNoTracking = true)
        => base.SingleQueryable(asNoTracking).Include(x => x.Category);
}
