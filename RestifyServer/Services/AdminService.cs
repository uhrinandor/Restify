using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.Models.Enums;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class AdminService(IRepository<Models.Admin> adminRepo, IUnitOfWork unitOfWork, IPasswordHasher<Models.Admin> passwordHasher, IMapper mapper) : IAdminService
{
    public async Task<List<Admin>> List(FindAdmin query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Admin>();
        if (query.Id != null) p = p.And<Models.Admin>(a => a.Id == query.Id);
        if (query.Username != null) p = p.And<Models.Admin>(a => a.Username == query.Username);
        if (query.AccessLevel != null) p = p.And(a => a.AccessLevel == query.AccessLevel);
        var list = await adminRepo.ListAsync(p, ct, asNoTracking: true);
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
        adminRepo.Add(dbAdmin);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.Map<Admin>(dbAdmin);
    }

    public async Task<Admin?> FindById(Guid id, CancellationToken ct = default)
    {
        var dbAdmin = await LoadAdminAsync(id, ct);
        return mapper.Map<Admin>(dbAdmin);
    }

    public async Task<Admin?> Update(Guid id, UpdateAdmin admin, CancellationToken ct = default)
    {
        var dbAdmin = await LoadAdminAsync(id, ct);

        if (admin.Username != null) dbAdmin.Username = admin.Username;
        if (admin.WriteMode is bool writeMode) dbAdmin.AccessLevel = writeMode ? Permission.Write : Permission.Read;
        await unitOfWork.SaveChangesAsync(ct);
        return mapper.Map<Admin>(dbAdmin);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbAdmin = await LoadAdminAsync(id, ct);
        adminRepo.Remove(dbAdmin);
        await unitOfWork.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> UpdatePassword(Guid id, UpdateAdminPassword credentials, CancellationToken ct = default)
    {
        var dbAdmin = await LoadAdminAsync(id, ct);

        if (passwordHasher.VerifyHashedPassword(dbAdmin, dbAdmin.Password, credentials.OldPassword) == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException();
        dbAdmin.Password = passwordHasher.HashPassword(dbAdmin, credentials.NewPassword);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    private async Task<Models.Admin> LoadAdminAsync(Guid id, CancellationToken ct = default)
    {
        var dbAdmin = await adminRepo.FirstOrDefaultAsync(a => a.Id == id, ct, false) ?? throw new NotFoundException(id, typeof(Admin));
        return dbAdmin;
    }
}
