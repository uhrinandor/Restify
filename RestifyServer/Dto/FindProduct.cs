namespace RestifyServer.Dto;

public record FindProduct(
    Guid? Id,
    string? Name,
    string? Description,
    decimal? Price,
    FindEntity? Category
    );
