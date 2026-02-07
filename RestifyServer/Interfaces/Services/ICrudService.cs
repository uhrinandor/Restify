namespace RestifyServer.Interfaces.Services;

public interface ICrudService<TContract, in TCreateContract, in TUpdateContract, in TFindContract>
{
    Task<List<TContract>> List(TFindContract query, CancellationToken ct = default);
    Task<TContract> Create(TCreateContract data, CancellationToken ct = default);
    Task<TContract?> FindById(Guid id, CancellationToken ct = default);
    Task<TContract?> Update(Guid id, TUpdateContract data, CancellationToken ct = default);
    Task<bool> Delete(Guid id, CancellationToken ct = default);
}
