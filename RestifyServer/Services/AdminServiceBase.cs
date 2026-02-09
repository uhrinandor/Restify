using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.Models.Enums;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class AdminServiceBase(IRepository<Models.Admin> adminRepo, IEntityService<Models.Admin> entityService, IPasswordHasher<Models.Admin> passwordHasher, IMapper mapper) :
    CrudServiceBase<Models.Admin, Admin, CreateAdmin, UpdateAdmin, FindAdmin>(adminRepo, entityService, mapper), IAdminService
{
    protected override Task<Models.Admin> CreateEntity(CreateAdmin data, CancellationToken ct = default)
    {
        Models.Admin dbAdmin = new()
        {
            Username = data.Username,
            AccessLevel = data.WriteMode ? Permission.Write : Permission.Read,
        };
        dbAdmin.Password = passwordHasher.HashPassword(dbAdmin, data.Password);

        return Task.FromResult(dbAdmin);
    }

    protected override Task SetEntityProperties(Models.Admin entity, UpdateAdmin data, CancellationToken ct = default)
    {
        if (data.Username != null) entity.Username = data.Username;
        if (data.WriteMode is bool writeMode) entity.AccessLevel = writeMode ? Permission.Write : Permission.Read;

        return Task.CompletedTask;
    }

    protected override Expression<Func<Models.Admin, bool>> CreateQuery(FindAdmin query)
    {
        var p = Predicate.True<Models.Admin>();
        if (query.Id != null) p = p.And(a => a.Id == query.Id);
        if (query.Username != null) p = p.And(a => a.Username == query.Username);
        if (query.AccessLevel != null) p = p.And(a => a.AccessLevel == query.AccessLevel);
        return p;
    }

    public async Task<bool> UpdatePassword(Guid id, UpdatePassword credentials, CancellationToken ct = default)
    {
        var dbAdmin = await EntityService.LoadEntityAsync(id, ct);

        if (passwordHasher.VerifyHashedPassword(dbAdmin, dbAdmin.Password, credentials.OldPassword) == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException();
        dbAdmin.Password = passwordHasher.HashPassword(dbAdmin, credentials.NewPassword);
        return true;
    }
}
