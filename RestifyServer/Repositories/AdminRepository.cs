using Microsoft.EntityFrameworkCore;
using RestifyServer.Models;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class AdminRepository : Repository<Admin>
{
    public AdminRepository(RestifyContext db): base(db) {}
}
