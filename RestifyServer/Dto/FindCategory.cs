using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record FindCategory(
    Guid? Id,
    string? Name,
    FindEntity? Parent
);
