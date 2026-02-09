using Microsoft.EntityFrameworkCore;
using RestifyServer.Data;
using RestifyServer.Models;

namespace RestifyServer.Repositories;

public class OrderRepository(RestifyContext db) : Repository<Order>(db)
{
    protected override IQueryable<Order> ListQueryable(bool asNoTracking = true) =>
        base.ListQueryable(asNoTracking).Include(x => x.Product);

    protected override IQueryable<Order> SingleQueryable(bool asNoTracking = true)
    => base.ListQueryable(asNoTracking)
    .Include(x => x.Product)
    .Include(x => x.Invoice)
    .Include(x => x.Invoice.Table)
    .Include(x => x.Invoice.Waiter);
}
