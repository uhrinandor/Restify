using System.Linq.Expressions;
using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class ProductServiceBase(IRepository<Models.Product> productRepo, IEntityService<Models.Product> entityService, IEntityService<Models.Category> categoryEntityService, IMapper mapper) :
    CrudServiceBase<Models.Product, Product, CreateProduct, UpdateProduct, FindProduct>(productRepo, entityService, mapper), IProductService
{
    protected override async Task<Models.Product> CreateEntity(CreateProduct data, CancellationToken ct = default)
    {
        var dbCategory = await categoryEntityService.LoadEntityAsync(data.Category.Id, ct);

        var dbProduct = new Models.Product()
        {
            Name = data.Name,
            Description = data.Description ?? "",
            Price = data.Price,
            Category = dbCategory
        };
        return dbProduct;
    }

    protected override async Task SetEntityProperties(Models.Product entity, UpdateProduct data, CancellationToken ct = default)
    {
        if (!string.IsNullOrEmpty(data.Name)) entity.Name = data.Name;
        if (!string.IsNullOrEmpty(data.Description)) entity.Description = data.Description;
        if (data.Price != null) entity.Price = data.Price ?? entity.Price;
        if (data.Category != null) entity.Category = await categoryEntityService.LoadEntityAsync(data.Category.Id, ct);
    }

    protected override Expression<Func<Models.Product, bool>> CreateQuery(FindProduct query)
    {
        var p = Predicate.True<Models.Product>();
        if (!string.IsNullOrEmpty(query.Name)) p = p.And(x => x.Name == query.Name);
        if (!string.IsNullOrEmpty(query.Description)) p = p.And(x => x.Description.Contains(query.Description));
        if (query.Price != null) p = p.And(x => x.Price == query.Price);
        if (query.Category != null) p = p.And(x => x.Category.Id == query.Category.Id);

        return p;
    }
}
