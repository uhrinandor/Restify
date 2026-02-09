using Microsoft.EntityFrameworkCore;
using RestifyServer.Data;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class InvoiceRepository(RestifyContext db) : Repository<Invoice>(db)
{
    protected override IQueryable<Invoice> SingleQueryable(bool asNoTracking = true)
        => base.SingleQueryable(asNoTracking).Include(x => x.Table).Include(x => x.Waiter).Include(x => x.Orders);

    protected override IQueryable<Invoice> ListQueryable(bool asNoTracking = true)
    => base.ListQueryable(asNoTracking).Include(x => x.Table).Include(x => x.Waiter);

}
