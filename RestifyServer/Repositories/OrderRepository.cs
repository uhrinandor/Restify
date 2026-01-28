using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class OrderRepository : Repository<Order>
{
    public OrderRepository(RestifyContext db): base(db) {}
}
