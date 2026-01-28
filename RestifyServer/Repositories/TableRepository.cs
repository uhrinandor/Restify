using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class TableRepository : Repository<Table>
{
    public TableRepository(RestifyContext db): base(db) {}
}
