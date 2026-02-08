using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class WaiterService(IRepository<Models.Waiter> waiterRepo, IUnitOfWork unitOfWork, IPasswordHasher<Models.Waiter> passwordHasher, IMapper mapper) : BaseService<Models.Waiter>(waiterRepo), IWaiterService
{
    public async Task<List<Waiter>> List(FindWaiter query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Waiter>();
        if (!string.IsNullOrEmpty(query.Username)) p = p.And(a => a.Username == query.Username);
        if (!string.IsNullOrEmpty(query.Name)) p = p.And(a => a.Name == query.Name);
        if (query.id != null) p = p.And(a => a.Id == query.id);
        var list = await EntityRepository.ListAsync(p, ct);

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
        EntityRepository.Add(dbWaiter);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.Map<Waiter>(dbWaiter);
    }

    public async Task<Waiter?> FindById(Guid id, CancellationToken ct = default)
    {
        var dbWaiter = await LoadEntity(id, ct);

        return mapper.Map<Waiter>(dbWaiter);
    }

    public async Task<Waiter?> Update(Guid id, UpdateWaiter data, CancellationToken ct = default)
    {
        var dbWaiter = await LoadEntityAsync(id, ct);

        if (!string.IsNullOrEmpty(data.Username)) dbWaiter.Username = data.Username;
        if (!string.IsNullOrEmpty(data.Name)) dbWaiter.Name = data.Name;
        await unitOfWork.SaveChangesAsync(ct);
        return mapper.Map<Waiter>(dbWaiter);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbWaiter = await LoadEntityAsync(id, ct);

        EntityRepository.Remove(dbWaiter);

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdatePassword(Guid id, UpdatePassword credentials, CancellationToken ct = default)
    {
        var dbWaiter = await LoadEntityAsync(id, ct);

        if (passwordHasher.VerifyHashedPassword(dbWaiter, dbWaiter.Password, credentials.OldPassword) == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException();
        dbWaiter.Password = passwordHasher.HashPassword(dbWaiter, credentials.NewPassword);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
