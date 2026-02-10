using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record UpdateInvoice(
    FindEntity? Waiter,

    FindEntity? Table
);
