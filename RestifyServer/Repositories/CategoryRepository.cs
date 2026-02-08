using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RestifyServer.Data;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class CategoryRepository(RestifyContext db) : Repository<Category>(db)
{
    protected override IQueryable<Category> ListQueryable(bool asNoTracking = true) =>
        base.ListQueryable(asNoTracking).Include(x => x.Parent).Include(x => x.Children);

    protected override IQueryable<Category> SingleQueryable(bool asNoTracking = true)
        => base.SingleQueryable(asNoTracking).Include(x => x.Parent).Include(x => x.Children).Include(x => x.Products);
}
