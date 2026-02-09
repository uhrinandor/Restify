using System.Linq.Expressions;
using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class CategoryService(IRepository<Models.Category> categoryRepo, IEntityService<Models.Category> entityService, IMapper mapper) :
    CrudServiceBase<Models.Category, Category, CreateCategory, UpdateCategory, FindCategory>(categoryRepo, entityService, mapper), ICategoryService
{
    protected override async Task<Models.Category> CreateEntity(CreateCategory data, CancellationToken ct = default)
    {
        Models.Category? parent = null;
        if (data.Parent != null)
        {
            parent = await EntityService.LoadEntityAsync(data.Parent.Id, ct);
        }

        var dbCategory = new Models.Category()
        {
            Name = data.Name,
            Parent = parent
        };

        return dbCategory;
    }

    protected override async Task SetEntityProperties(Models.Category entity, UpdateCategory data, CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(data.Name)) entity.Name = data.Name;

        if (data.Parent != null) entity.Parent = await EntityService.LoadEntityAsync(data.Parent.Id, ct);
    }

    protected override Expression<Func<Models.Category, bool>> CreateQuery(FindCategory query)
    {
        var p = Predicate.True<Models.Category>();
        if (query.Id != null) p = p.And(a => a.Id == query.Id);
        if (!string.IsNullOrEmpty(query.Name)) p = p.And(a => a.Name == query.Name);
        if (query.Parent?.Id != null) p = p.And(a => a.Parent != null && a.Parent.Id == query.Parent.Id);
        if (query.Parent?.Name != null) p = p.And(a => a.Parent != null && a.Parent.Name == query.Parent.Name);

        return p;
    }
}
