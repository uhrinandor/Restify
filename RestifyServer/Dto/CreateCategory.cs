using System.ComponentModel.DataAnnotations;
using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record CreateCategory(
    [param: Required]
    string Name,
    Category? Parent
);
