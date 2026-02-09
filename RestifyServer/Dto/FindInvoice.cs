using RestifyServer.Models.Enums;

namespace RestifyServer.Dto;

public record FindInvoice(Guid? Id, FindWaiter? Waiter, FindTable? Table, PaymentType? Payment, bool? IsClosed);
