using RestifyServer.Dto;
using RestifyServer.TypeContracts;

namespace RestifyServer.Interfaces.Services;

public interface IWaiterService : ICrudService<Waiter, CreateWaiter, UpdateWaiter, FindWaiter>
{
    Task<bool> UpdatePassword(Guid id, UpdatePassword credentials, CancellationToken ct = default);
}
