using System.Linq.Expressions;
using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class TableServiceBase(IRepository<Models.Table> tableRepo, IEntityService<Models.Table> entityService, IMapper mapper) :
    CrudServiceBase<Models.Table, Table, CreateTable, UpdateTable, FindTable>(tableRepo, entityService, mapper), ITableService
{
    protected override Task<Models.Table> CreateEntity(CreateTable data, CancellationToken ct = default)
    {
        var dbTable = new Models.Table()
        {
            Number = data.Number
        };
        return Task.FromResult(dbTable);
    }

    protected override Task SetEntityProperties(Models.Table entity, UpdateTable data, CancellationToken ct = default)
    {
        if (data.Number != null) entity.Number = data.Number ?? entity.Number;

        return Task.CompletedTask;
    }

    protected override Expression<Func<Models.Table, bool>> CreateQuery(FindTable query)
    {
        var p = Predicate.True<Models.Table>();
        if (query.Id != null) p = p.And(x => x.Id == query.Id);
        if (query.Number != null) p = p.And(x => x.Number == query.Number);
        return p;
    }
}
