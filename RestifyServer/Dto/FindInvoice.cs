using RestifyServer.Models.Enums;

namespace RestifyServer.Dto;

public record FindInvoice(Guid? Id, FindEntity? Waiter, FindEntity? Table, PaymentType? Payment, bool? IsClosed);
