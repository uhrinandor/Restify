using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class InvoiceRepository : Repository<Invoice>
{
    public InvoiceRepository(RestifyContext db): base(db) {}
}
