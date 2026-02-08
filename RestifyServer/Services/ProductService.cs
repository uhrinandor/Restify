using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class ProductService(IRepository<Models.Category> categoryRepo, IRepository<Models.Product> productRepository, IMapper mapper, IUnitOfWork unitOfWork) : BaseService<Models.Product>(productRepository), IProductService
{
    public async Task<List<Product>> List(FindProduct query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Product>();
        if (!string.IsNullOrEmpty(query.Name)) p = p.And(x => x.Name == query.Name);
        if (!string.IsNullOrEmpty(query.Description)) p = p.And(x => x.Description.Contains(query.Description));
        if (query.Price != null) p = p.And(x => x.Price == query.Price);
        if (query.Category != null) p = p.And(x => x.Category.Id == query.Category.Id);
        var list = await EntityRepository.ListAsync(p, ct);

        return mapper.Map<List<Product>>(list);
    }

    public async Task<Product> Create(CreateProduct data, CancellationToken ct = default)
    {
        var dbCategory = await categoryRepo.GetByIdAsync(data.Category.Id, ct, false);
        if (dbCategory == null) throw new NotFoundException(data.Category.Id, typeof(Category));

        var dbProduct = new Models.Product()
        {
            Name = data.Name,
            Description = data.Description ?? "",
            Price = data.Price,
            Category = dbCategory
        };

        EntityRepository.Add(dbProduct);
        await unitOfWork.SaveChangesAsync(ct);

        return mapper.Map<Product>(dbProduct);
    }

    public async Task<Product?> FindById(Guid id, CancellationToken ct = default)
    {
        var dbProduct = await LoadEntity(id, ct);

        return mapper.Map<Product>(dbProduct);
    }

    public async Task<Product?> Update(Guid id, UpdateProduct data, CancellationToken ct = default)
    {
        var dbProduct = await LoadEntityAsync(id, ct);

        if (!string.IsNullOrEmpty(data.Name)) dbProduct.Name = data.Name;
        if (!string.IsNullOrEmpty(data.Description)) dbProduct.Description = data.Description;
        if (data.Price != null) dbProduct.Price = data.Price ?? dbProduct.Price;
        if (data.Category != null)
        {
            var dbCategory = await categoryRepo.GetByIdAsync(data.Category.Id, ct, false);
            if (dbCategory == null) throw new NotFoundException(data.Category.Id, typeof(Category));
            dbProduct.Category = dbCategory;
        }

        await unitOfWork.SaveChangesAsync(ct);
        return mapper.Map<Product>(dbProduct);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbProduct = await LoadEntityAsync(id, ct);

        EntityRepository.Remove(dbProduct);

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
