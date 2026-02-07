using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record FindCategory(
    Guid? Id,
    string? Name,
    FindParentCategory? Parent
);

public record FindParentCategory(
    Guid? Id,
    string? Name
);
