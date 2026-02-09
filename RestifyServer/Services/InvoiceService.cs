using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class InvoiceService(IRepository<Models.Invoice> invoiceRepo, IEntityService<Models.Invoice> entityService, IEntityService<Models.Waiter> waiterEntityService, IEntityService<Models.Table> tableEntityService, IMapper mapper) : IInvoiceService
{
    public async Task<List<Invoice>> List(FindInvoice query, CancellationToken ct = default)
    {
        var p = Predicate.True<Models.Invoice>();
        if (query.Id != null) p = p.And(x => x.Id == query.Id);
        if (query.Waiter?.Id != null) p = p.And(x => x.Waiter.Id == query.Waiter.Id);
        if (query.Table?.Id != null) p = p.And(x => x.Table.Id == query.Table.Id);
        if (query.Payment != null) p = p.And(x => x.Payment == query.Payment);
        if (query.IsClosed != null) p = p.And(x => (x.ClosedAt != null) == query.IsClosed);

        var list = await invoiceRepo.ListAsync(p, ct);
        return mapper.Map<List<Invoice>>(list);
    }

    public async Task<Invoice> Create(CreateInvoice data, CancellationToken ct = default)
    {
        var dbWaiter = await waiterEntityService.LoadEntityAsync(data.Waiter.Id, ct);
        var dbTable = await tableEntityService.LoadEntityAsync(data.Table.Id, ct);

        var invoice = new Models.Invoice()
        {
            Waiter = dbWaiter,
            Table = dbTable
        };

        invoiceRepo.Add(invoice);
        return mapper.Map<Invoice>(invoice);
    }

    public async Task<Invoice> FindById(Guid id, CancellationToken ct = default)
    {
        var dbInvoice = await entityService.LoadEntityAsync(id, ct);

        return mapper.Map<Invoice>(dbInvoice);
    }

    public async Task<Invoice> Update(Guid id, UpdateInvoice data, CancellationToken ct = default)
    {
        var dbInvoice = await entityService.LoadEntityAsync(id, ct);

        if (data.Waiter != null) dbInvoice.Waiter = await waiterEntityService.LoadEntityAsync(data.Waiter.Id, ct);
        if (data.Table != null) dbInvoice.Table = await tableEntityService.LoadEntityAsync(data.Table.Id, ct);

        return mapper.Map<Invoice>(dbInvoice);
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct = default)
    {
        var dbInvoice = await entityService.LoadEntityAsync(id, ct);

        invoiceRepo.Remove(dbInvoice);
        return true;
    }
}
