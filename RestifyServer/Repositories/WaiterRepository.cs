using Microsoft.EntityFrameworkCore;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class WaiterRepository : Repository<Waiter>
{
    public WaiterRepository(RestifyContext db) : base(db) { }
}
