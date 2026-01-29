using Microsoft.AspNetCore.Mvc;
using RestifyServer.Dto;
using RestifyServer.TypeContracts;

namespace RestifyServer.Interfaces.Services;

public interface IAdminService
{
    Task<List<Admin>> List(FindAdmin query, CancellationToken ct = default);
    Task<Admin> Create(CreateAdmin admin, CancellationToken ct = default);
    Task<Admin?> FindById(Guid id, CancellationToken ct = default);
    Task<Admin?> Update(Guid id, UpdateAdmin admin, CancellationToken ct = default);
    Task<bool> Delete(Guid id, CancellationToken ct = default);
    Task<bool> UpdatePassword(Guid id, UpdateAdminPassword credentials, CancellationToken ct = default);

}
