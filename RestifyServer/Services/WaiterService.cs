using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class WaiterService(IRepository<Models.Waiter> waiterRepo, IEntityService<Models.Waiter> entityService, IPasswordHasher<Models.Waiter> passwordHasher, IMapper mapper) : IWaiterService
{
    public async Task<List<Waiter>> List(FindWaiter query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Waiter>();
        if (!string.IsNullOrEmpty(query.Username)) p = p.And(a => a.Username == query.Username);
        if (!string.IsNullOrEmpty(query.Name)) p = p.And(a => a.Name == query.Name);
        if (query.Id != null) p = p.And(a => a.Id == query.Id);
        var list = await waiterRepo.ListAsync(p, ct);

        return mapper.Map<List<Waiter>>(list);
    }

    public Task<Waiter> Create(CreateWaiter waiter, CancellationToken ct = default)
    {
        var dbWaiter = new Models.Waiter()
        {
            Username = waiter.Username,
            Name = waiter.Name
        };
        dbWaiter.Password = passwordHasher.HashPassword(dbWaiter, waiter.Password);
        waiterRepo.Add(dbWaiter);

        var mapped = mapper.Map<Waiter>(dbWaiter);
        return Task.FromResult(mapped);
    }

    public async Task<Waiter> FindById(Guid id, CancellationToken ct = default)
    {
        var dbWaiter = await entityService.LoadEntity(id, ct);

        return mapper.Map<Waiter>(dbWaiter);
    }

    public async Task<Waiter> Update(Guid id, UpdateWaiter data, CancellationToken ct = default)
    {
        var dbWaiter = await entityService.LoadEntityAsync(id, ct);

        if (!string.IsNullOrEmpty(data.Username)) dbWaiter.Username = data.Username;
        if (!string.IsNullOrEmpty(data.Name)) dbWaiter.Name = data.Name;
        return mapper.Map<Waiter>(dbWaiter);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbWaiter = await entityService.LoadEntityAsync(id, ct);

        waiterRepo.Remove(dbWaiter);

        return true;
    }

    public async Task<bool> UpdatePassword(Guid id, UpdatePassword credentials, CancellationToken ct = default)
    {
        var dbWaiter = await entityService.LoadEntityAsync(id, ct);

        if (passwordHasher.VerifyHashedPassword(dbWaiter, dbWaiter.Password, credentials.OldPassword) == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException();
        dbWaiter.Password = passwordHasher.HashPassword(dbWaiter, credentials.NewPassword);
        return true;
    }
}
