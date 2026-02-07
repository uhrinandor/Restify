using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record UpdateCategory(
    string? Name,
    Category? Parent
);
