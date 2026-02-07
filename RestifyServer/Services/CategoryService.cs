using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class CategoryService(IRepository<Models.Category> categoryRepo, IMapper mapper, IUnitOfWork unitOfWork) : ICategoryService
{
    public async Task<List<Category>> List(FindCategory query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Category>();
        if (query.Id != null) p = p.And(a => a.Id == query.Id);
        if (!string.IsNullOrEmpty(query.Name)) p = p.And(a => a.Name == query.Name);
        if (query.Parent?.Id != null) p = p.And(a => a.Parent != null && a.Parent.Id == query.Parent.Id);
        if (query.Parent?.Name != null) p = p.And(a => a.Parent != null && a.Parent.Name == query.Parent.Name);

        var list = await categoryRepo.ListAsync(p, ct);

        return mapper.Map<List<Category>>(list);
    }

    public async Task<Category> Create(CreateCategory data, CancellationToken ct = default)
    {
        Models.Category? parent = null;

        if (data.Parent != null)
        {
            parent = await LoadCategoryAsync(data.Parent.Id, ct);
        }

        var dbCategory = new Models.Category()
        {
            Name = data.Name,
            Parent = parent
        };

        categoryRepo.Add(dbCategory);

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.Map<Category>(dbCategory);
    }

    public async Task<Category?> FindById(Guid id, CancellationToken ct = default)
    {
        var dbCategory = await LoadCategoryAsync(id, ct);

        return mapper.Map<Category>(dbCategory);
    }

    public async Task<Category?> Update(Guid id, UpdateCategory data, CancellationToken ct = default)
    {
        var dbCategory = await LoadCategoryAsync(id, ct);

        if (!string.IsNullOrEmpty(data.Name)) dbCategory.Name = data.Name;

        if (data.Parent != null)
        {
            var parent = await LoadCategoryAsync(data.Parent.Id, ct);
            dbCategory.Parent = parent;
        }

        await unitOfWork.SaveChangesAsync(ct);

        return mapper.Map<Category>(dbCategory);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbCategory = await LoadCategoryAsync(id, ct);

        categoryRepo.Remove(dbCategory);

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    private async Task<Models.Category> LoadCategoryAsync(Guid id, CancellationToken ct = default)
    {
        var dbCategory = await categoryRepo.GetByIdAsync(id, ct, false) ?? throw new NotFoundException(id, typeof(Category));

        return dbCategory;
    }
}
