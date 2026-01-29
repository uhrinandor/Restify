using Microsoft.EntityFrameworkCore;
using RestifyServer.Interfaces;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Repository;

namespace RestifyServer.Repositories;

public class UnitOfWork(RestifyContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
