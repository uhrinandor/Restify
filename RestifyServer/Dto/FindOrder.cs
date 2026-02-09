using RestifyServer.Models.Enums;

namespace RestifyServer.Dto;

public record FindOrder(Guid? Id, FindInvoice? Invoice, OrderStatus? Status, FindProduct? Product);
