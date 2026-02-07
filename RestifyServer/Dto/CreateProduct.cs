using System.ComponentModel.DataAnnotations;
using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record CreateProduct(
    [param: Required]
    string Name,
    string? Description,
    [param: Required]
    decimal Price,
    Category Category
    );
