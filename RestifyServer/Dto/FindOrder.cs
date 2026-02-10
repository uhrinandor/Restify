using RestifyServer.Models.Enums;

namespace RestifyServer.Dto;

public record FindOrder(Guid? Id, FindEntity? Invoice, OrderStatus? Status, FindEntity? Product);
