using Microsoft.AspNetCore.Mvc;
using RestifyServer.Dto;
using RestifyServer.TypeContracts;

namespace RestifyServer.Interfaces.Services;

public interface IAdminService : ICrudService<Admin, CreateAdmin, UpdateAdmin, FindAdmin>
{
    Task<bool> UpdatePassword(Guid id, UpdatePassword credentials, CancellationToken ct = default);
}
