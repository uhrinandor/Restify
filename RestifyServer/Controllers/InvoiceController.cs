using Microsoft.AspNetCore.Mvc;
using RestifyServer.Dto;
using RestifyServer.Interfaces.Services;
using RestifyServer.TypeContracts;

namespace RestifyServer.Controllers;

public class InvoiceController(IInvoiceService invoiceService) : CrudController<Invoice, CreateInvoice, UpdateInvoice, FindInvoice>(invoiceService)
{
    [HttpPut("{id}/close")]
    public ActionResult Close([FromRoute] Guid id, [FromBody] CloseInvoice closeInvoice) => throw new NotImplementedException();
}
