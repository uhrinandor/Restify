using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class TableService(IRepository<Models.Table> tableRepo, IEntityService<Models.Table> entityService, IMapper mapper) : ITableService
{
    public async Task<List<Table>> List(FindTable query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Table>();
        if (query.Id != null) p = p.And(x => x.Id == query.Id);
        if (query.Number != null) p = p.And(x => x.Number == query.Number);

        var list = await tableRepo.ListAsync(p, ct);
        return mapper.Map<List<Table>>(list);
    }

    public Task<Table> Create(CreateTable data, CancellationToken ct = default)
    {
        var dbTable = new Models.Table()
        {
            Number = data.Number
        };

        tableRepo.Add(dbTable);
        var mapped = mapper.Map<Table>(dbTable);
        return Task.FromResult(mapped);
    }

    public async Task<Table> FindById(Guid id, CancellationToken ct = default)
    {
        var dbTable = await entityService.LoadEntity(id, ct);
        return mapper.Map<Table>(dbTable);
    }

    public async Task<Table> Update(Guid id, UpdateTable data, CancellationToken ct = default)
    {
        var dbTable = await entityService.LoadEntityAsync(id, ct);
        if (data.Number != null) dbTable.Number = data.Number ?? dbTable.Number;

        return mapper.Map<Table>(dbTable);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbTable = await entityService.LoadEntityAsync(id, ct);
        tableRepo.Remove(dbTable);

        return true;
    }
}
