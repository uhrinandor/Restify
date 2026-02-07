using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record UpdateProduct(
    string? Name,
    string? Description,
    decimal? Price,
    Category? Category
    );
