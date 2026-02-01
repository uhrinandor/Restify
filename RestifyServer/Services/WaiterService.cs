using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class WaiterService(IRepository<Models.Waiter> waiterRepo, IUnitOfWork unitOfWork, IPasswordHasher<Models.Waiter> passwordHasher, IMapper mapper) : IWaiterService
{
    public async Task<List<Waiter>> List(FindWaiter query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Waiter>();
        if (!string.IsNullOrEmpty(query.Username)) p = p.And(a => a.Username == query.Username);
        if (!string.IsNullOrEmpty(query.Name)) p = p.And(a => a.Name == query.Name);
        if (query.id != null) p = p.And<Models.Waiter>(a => a.Id == query.id);
        var list = await waiterRepo.ListAsync(p, ct);

        return mapper.Map<List<Waiter>>(list);
    }

    public async Task<Waiter> Create(CreateWaiter waiter, CancellationToken ct = default)
    {
        var dbWaiter = new Models.Waiter()
        {
            Username = waiter.Username,
            Name = waiter.Name
        };
        dbWaiter.Password = passwordHasher.HashPassword(dbWaiter, waiter.Password);
        waiterRepo.Add(dbWaiter);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.Map<Waiter>(dbWaiter);
    }

    public async Task<Waiter?> FindById(Guid id, CancellationToken ct = default)
    {
        var dbWaiter = await LoadWaiterAsync(id, ct);

        return mapper.Map<Waiter>(dbWaiter);
    }

    public async Task<Waiter?> Update(Guid id, UpdateWaiter waiter, CancellationToken ct = default)
    {
        var dbWaiter = await LoadWaiterAsync(id, ct);

        if (!string.IsNullOrEmpty(waiter.Username)) dbWaiter.Username = waiter.Username;
        if (!string.IsNullOrEmpty(waiter.Name)) dbWaiter.Name = waiter.Name;
        await unitOfWork.SaveChangesAsync(ct);
        return mapper.Map<Waiter>(dbWaiter);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbWaiter = await LoadWaiterAsync(id, ct);

        waiterRepo.Remove(dbWaiter);

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdatePassword(Guid id, UpdatePassword credentials, CancellationToken ct = default)
    {
        var dbWaiter = await LoadWaiterAsync(id, ct);

        if (passwordHasher.VerifyHashedPassword(dbWaiter, dbWaiter.Password, credentials.OldPassword) == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException();
        dbWaiter.Password = passwordHasher.HashPassword(dbWaiter, credentials.NewPassword);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    private async Task<Models.Waiter> LoadWaiterAsync(Guid id, CancellationToken ct = default)
    {
        var dbWaiter = await waiterRepo.FirstOrDefaultAsync(a => a.Id == id, ct, false) ?? throw new NotFoundException(id, typeof(Waiter));

        return dbWaiter;
    }
}
