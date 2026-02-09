using RestifyServer.Dto;
using RestifyServer.TypeContracts;

namespace RestifyServer.Interfaces.Services;

public interface IInvoiceService : ICrudService<Invoice, CreateInvoice, UpdateInvoice, FindInvoice>
{

}
