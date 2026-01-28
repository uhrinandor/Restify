using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class ProductRepository : Repository<Product>
{
    public ProductRepository(RestifyContext db): base(db) {}
}
