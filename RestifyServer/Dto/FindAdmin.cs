using RestifyServer.Models.Enums;

namespace RestifyServer.Dto;

public record FindAdmin(Guid? Id, string? Username, Permission? AccessLevel);
