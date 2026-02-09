using RestifyServer.Models;

namespace RestifyServer.Interfaces.Services;

public interface IEntityService<TEntity> where TEntity : Entity
{
    public Task<TEntity> LoadEntityAsync(Guid id, CancellationToken ct = default);
    public Task<TEntity> LoadEntity(Guid id, CancellationToken ct = default);
}
