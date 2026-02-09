using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class WaiterService(IRepository<Models.Waiter> waiterRepo, IEntityService<Models.Waiter> entityService, IPasswordHasher<Models.Waiter> passwordHasher, IMapper mapper) :
    CrudServiceBase<Models.Waiter, Waiter, CreateWaiter, UpdateWaiter, FindWaiter>(waiterRepo, entityService, mapper), IWaiterService
{
    protected override Task<Models.Waiter> CreateEntity(CreateWaiter data, CancellationToken ct = default)
    {
        var dbWaiter = new Models.Waiter()
        {
            Username = data.Username,
            Name = data.Name
        };
        dbWaiter.Password = passwordHasher.HashPassword(dbWaiter, data.Password);
        return Task.FromResult(dbWaiter);
    }

    protected override Task SetEntityProperties(Models.Waiter entity, UpdateWaiter data, CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(data.Username)) entity.Username = data.Username;
        if (!string.IsNullOrEmpty(data.Name)) entity.Name = data.Name;

        return Task.CompletedTask;
    }

    protected override Expression<Func<Models.Waiter, bool>> CreateQuery(FindWaiter query)
    {
        var p = Predicate.True<Models.Waiter>();
        if (!string.IsNullOrEmpty(query.Username)) p = p.And(a => a.Username == query.Username);
        if (!string.IsNullOrEmpty(query.Name)) p = p.And(a => a.Name == query.Name);
        if (query.Id != null) p = p.And(a => a.Id == query.Id);
        return p;
    }

    public async Task<bool> UpdatePassword(Guid id, UpdatePassword credentials, CancellationToken ct = default)
    {
        var dbWaiter = await EntityService.LoadEntityAsync(id, ct);

        if (passwordHasher.VerifyHashedPassword(dbWaiter, dbWaiter.Password, credentials.OldPassword) == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException();
        dbWaiter.Password = passwordHasher.HashPassword(dbWaiter, credentials.NewPassword);
        return true;
    }
}
