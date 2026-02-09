using RestifyServer.Models.Enums;
using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record UpdateOrder(
    FindEntity? Product,
    FindEntity? Invoice,
    OrderStatus? Status
);
