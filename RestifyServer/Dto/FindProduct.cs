namespace RestifyServer.Dto;

public record FindProduct(
    string? Name,
    string? Description,
    decimal? Price,
    FindCategory? Category
    );
