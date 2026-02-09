using System.Linq.Expressions;
using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class InvoiceService(IRepository<Models.Invoice> invoiceRepo, IEntityService<Models.Invoice> entityService, IEntityService<Models.Waiter> waiterEntityService, IEntityService<Models.Table> tableEntityService, IMapper mapper) :
    CrudServiceBase<Models.Invoice, Invoice, CreateInvoice, UpdateInvoice, FindInvoice>(invoiceRepo, entityService, mapper), IInvoiceService
{
    protected override async Task<Models.Invoice> CreateEntity(CreateInvoice data, CancellationToken ct = default)
    {
        var dbWaiter = await waiterEntityService.LoadEntityAsync(data.Waiter.Id, ct);
        var dbTable = await tableEntityService.LoadEntityAsync(data.Table.Id, ct);

        var invoice = new Models.Invoice()
        {
            Waiter = dbWaiter,
            Table = dbTable
        };

        return invoice;
    }

    protected override async Task SetEntityProperties(Models.Invoice entity, UpdateInvoice data, CancellationToken ct = default)
    {
        if (data.Waiter != null) entity.Waiter = await waiterEntityService.LoadEntityAsync(data.Waiter.Id, ct);
        if (data.Table != null) entity.Table = await tableEntityService.LoadEntityAsync(data.Table.Id, ct);
    }

    protected override Expression<Func<Models.Invoice, bool>> CreateQuery(FindInvoice query)
    {
        var p = Predicate.True<Models.Invoice>();
        if (query.Id != null) p = p.And(x => x.Id == query.Id);
        if (query.Waiter?.Id != null) p = p.And(x => x.Waiter.Id == query.Waiter.Id);
        if (query.Table?.Id != null) p = p.And(x => x.Table.Id == query.Table.Id);
        if (query.Payment != null) p = p.And(x => x.Payment == query.Payment);
        if (query.IsClosed != null) p = p.And(x => (x.ClosedAt != null) == query.IsClosed);

        return p;
    }
}
