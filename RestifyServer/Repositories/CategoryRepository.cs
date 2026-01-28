using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class CategoryRepository : Repository<Category>
{
    public CategoryRepository(RestifyContext db) : base(db) { }
}
