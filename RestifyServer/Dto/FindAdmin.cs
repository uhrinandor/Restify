using RestifyServer.Models.Enums;

namespace RestifyServer.Dto;

public record FindAdmin(Guid? Id, String? Username, Permission? AccessLevel);
