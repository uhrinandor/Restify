using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class TableService(IRepository<Models.Table> tableRepo, IMapper mapper, IUnitOfWork unitOfWork) : BaseService<Models.Table>(tableRepo), ITableService
{
    public async Task<List<Table>> List(FindTable query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Table>();
        if (query.Id != null) p = p.And(x => x.Id == query.Id);
        if (query.Number != null) p = p.And(x => x.Number == query.Number);

        var list = await EntityRepository.ListAsync(p, ct);
        return mapper.Map<List<Table>>(list);
    }

    public async Task<Table> Create(CreateTable data, CancellationToken ct = default)
    {
        var dbTable = new Models.Table()
        {
            Number = data.Number
        };

        EntityRepository.Add(dbTable);
        await unitOfWork.SaveChangesAsync(ct);
        return mapper.Map<Table>(dbTable);
    }

    public async Task<Table?> FindById(Guid id, CancellationToken ct = default)
    {
        var dbTable = await LoadEntity(id, ct);
        return mapper.Map<Table>(dbTable);
    }

    public async Task<Table?> Update(Guid id, UpdateTable data, CancellationToken ct = default)
    {
        var dbTable = await LoadEntityAsync(id, ct);
        if (data.Number != null) dbTable.Number = data.Number ?? dbTable.Number;

        await unitOfWork.SaveChangesAsync(ct);
        return mapper.Map<Table>(dbTable);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbTable = await LoadEntityAsync(id, ct);
        EntityRepository.Remove(dbTable);

        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }
}
