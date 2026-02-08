using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.Models.Enums;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class AdminService(IRepository<Models.Admin> adminRepo, IUnitOfWork unitOfWork, IPasswordHasher<Models.Admin> passwordHasher, IMapper mapper) : BaseService<Models.Admin>(adminRepo), IAdminService
{
    public async Task<List<Admin>> List(FindAdmin query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Admin>();
        if (query.Id != null) p = p.And(a => a.Id == query.Id);
        if (query.Username != null) p = p.And(a => a.Username == query.Username);
        if (query.AccessLevel != null) p = p.And(a => a.AccessLevel == query.AccessLevel);
        var list = await EntityRepository.ListAsync(p, ct, asNoTracking: true);
        return mapper.Map<List<Admin>>(list);
    }

    public async Task<Admin> Create(CreateAdmin admin, CancellationToken ct = default)
    {
        Models.Admin dbAdmin = new()
        {
            Username = admin.Username,
            AccessLevel = admin.WriteMode ? Permission.Write : Permission.Read,
        };
        dbAdmin.Password = passwordHasher.HashPassword(dbAdmin, admin.Password);
        EntityRepository.Add(dbAdmin);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.Map<Admin>(dbAdmin);
    }

    public async Task<Admin?> FindById(Guid id, CancellationToken ct = default)
    {
        var dbAdmin = await LoadEntity(id, ct);
        return mapper.Map<Admin>(dbAdmin);
    }

    public async Task<Admin?> Update(Guid id, UpdateAdmin data, CancellationToken ct = default)
    {
        var dbAdmin = await LoadEntityAsync(id, ct);

        if (data.Username != null) dbAdmin.Username = data.Username;
        if (data.WriteMode is bool writeMode) dbAdmin.AccessLevel = writeMode ? Permission.Write : Permission.Read;
        await unitOfWork.SaveChangesAsync(ct);
        return mapper.Map<Admin>(dbAdmin);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbAdmin = await LoadEntityAsync(id, ct);
        EntityRepository.Remove(dbAdmin);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> UpdatePassword(Guid id, UpdatePassword credentials, CancellationToken ct = default)
    {
        var dbAdmin = await LoadEntityAsync(id, ct);

        if (passwordHasher.VerifyHashedPassword(dbAdmin, dbAdmin.Password, credentials.OldPassword) == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException();
        dbAdmin.Password = passwordHasher.HashPassword(dbAdmin, credentials.NewPassword);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
