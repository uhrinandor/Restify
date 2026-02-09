using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record UpdateInvoice(
    Waiter? Waiter,

    Table? Table
);
