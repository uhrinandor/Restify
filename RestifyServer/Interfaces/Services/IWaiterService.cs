using RestifyServer.Dto;
using RestifyServer.TypeContracts;

namespace RestifyServer.Interfaces.Services;

public interface IWaiterService
{
    Task<List<Waiter>> List(FindWaiter query, CancellationToken ct = default);
    Task<Waiter> Create(CreateWaiter waiter, CancellationToken ct = default);
    Task<Waiter?> FindById(Guid id, CancellationToken ct = default);
    Task<Waiter?> Update(Guid id, UpdateWaiter waiter, CancellationToken ct = default);
    Task<bool> Delete(Guid id, CancellationToken ct = default);
    Task<bool> UpdatePassword(Guid id, UpdatePassword credentials, CancellationToken ct = default);
}
