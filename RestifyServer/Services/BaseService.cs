using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Models;

namespace RestifyServer.Services;

public abstract class BaseService<TEntity>(IRepository<TEntity> repository) where TEntity : Entity
{
    protected readonly IRepository<TEntity> EntityRepository = repository;
    protected async Task<TEntity> LoadEntityAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await EntityRepository.GetByIdAsync(id, ct, false) ?? throw new NotFoundException(id, typeof(TEntity));
        return entity;
    }

    protected async Task<TEntity> LoadEntity(Guid id, CancellationToken ct = default)
    {
        var entity = await EntityRepository.GetByIdAsync(id, ct) ?? throw new NotFoundException(id, typeof(TEntity));
        return entity;
    }
}
