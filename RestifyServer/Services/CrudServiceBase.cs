using System.Linq.Expressions;
using AutoMapper;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.Models;

namespace RestifyServer.Services;

public abstract class CrudServiceBase<TEntityModel, TContract, TCreate, TUpdate, TFind>(IRepository<TEntityModel> entityRepo, IEntityService<TEntityModel> entityService, IMapper mapper) : ICrudService<TContract, TCreate, TUpdate, TFind> where TEntityModel : Entity
{
    protected readonly IRepository<TEntityModel> EntityRepo = entityRepo;
    protected readonly IEntityService<TEntityModel> EntityService = entityService;
    protected abstract Task<TEntityModel> CreateEntity(TCreate data, CancellationToken ct = default);
    protected abstract Task SetEntityProperties(TEntityModel entity, TUpdate data, CancellationToken ct = default);
    protected abstract Expression<Func<TEntityModel, bool>> CreateQuery(TFind query);
    public async Task<List<TContract>> List(TFind query, CancellationToken ct = default)
    {
        var p = CreateQuery(query);
        var list = await EntityRepo.ListAsync(p, ct);
        return mapper.Map<List<TContract>>(list);
    }

    public async Task<TContract> Create(TCreate data, CancellationToken ct = default)
    {
        var entity = await CreateEntity(data, ct);

        EntityRepo.Add(entity);
        return mapper.Map<TContract>(entity);
    }

    public async Task<TContract> FindById(Guid id, CancellationToken ct = default)
    {
        var entity = await EntityService.LoadEntity(id, ct);

        return mapper.Map<TContract>(entity);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var entity = await EntityService.LoadEntityAsync(id, ct);
        EntityRepo.Remove(entity);
        return true;
    }

    public async Task<TContract> Update(Guid id, TUpdate data, CancellationToken ct = default)
    {
        var entity = await EntityService.LoadEntityAsync(id, ct);
        await SetEntityProperties(entity, data, ct);
        return mapper.Map<TContract>(entity);
    }
}
