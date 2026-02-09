using System.Linq.Expressions;
using AutoMapper;
using RestifyServer.Dto;
using RestifyServer.Exceptions;
using RestifyServer.Interfaces.Repositories;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;
using RestifyServer.Utils;

namespace RestifyServer.Services;

public class OrderService(IRepository<Models.Order> orderRepo, IEntityService<Models.Order> entityService, IEntityService<Models.Product> productEntityService, IEntityService<Models.Invoice> invoiceEntityService, IMapper mapper) :
    CrudServiceBase<Models.Order, Order, CreateOrder, UpdateOrder, FindOrder>(orderRepo, entityService, mapper), IOrderService
{
    protected override async Task<Models.Order> CreateEntity(CreateOrder data, CancellationToken ct = default)
    {
        var dbProduct = await productEntityService.LoadEntityAsync(data.Product.Id, ct);
        var dbInvoice = await invoiceEntityService.LoadEntityAsync(data.Invoice.Id, ct);
        if (dbInvoice.ClosedAt != null) throw new BuisnessLogicException("Invoice cannot be closed!", null);
        var dbOrder = new Models.Order()
        {
            Product = dbProduct,
            Invoice = dbInvoice
        };

        return dbOrder;
    }

    protected override async Task SetEntityProperties(Models.Order entity, UpdateOrder data, CancellationToken ct = default)
    {
        if (data.Status != null) entity.Status = data.Status ?? entity.Status;
        if (data.Invoice != null)
        {
            var dbInvoice = await invoiceEntityService.LoadEntityAsync(data.Invoice.Id, ct);
            if (dbInvoice.ClosedAt != null) throw new BuisnessLogicException("Invoice cannot be closed!", null);
            entity.Invoice = dbInvoice;
        }
        if (data.Product != null) entity.Product = await productEntityService.LoadEntityAsync(data.Product.Id, ct);
    }

    protected override Expression<Func<Models.Order, bool>> CreateQuery(FindOrder query)
    {
        var p = Predicate.True<Models.Order>();

        if (query.Id != null) p = p.And(x => x.Id == query.Id);
        if (query.Status != null) p = p.And(x => x.Status == query.Status);
        if (query.Product?.Id != null) p = p.And(x => x.Product.Id == query.Product.Id);
        return p;
    }
}
