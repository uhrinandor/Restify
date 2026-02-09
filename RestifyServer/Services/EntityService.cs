using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.Models;

namespace RestifyServer.Services;

public class EntityService<TEntity>(IRepository<TEntity> repository) : IEntityService<TEntity> where TEntity : Entity
{
    public async Task<TEntity> LoadEntityAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct, false) ?? throw new NotFoundException(id, typeof(TEntity));
        return entity;
    }

    public async Task<TEntity> LoadEntity(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.GetByIdAsync(id, ct) ?? throw new NotFoundException(id, typeof(TEntity));
        return entity;
    }
}
