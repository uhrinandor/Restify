using System.ComponentModel.DataAnnotations;
using RestifyServer.Models.Enums;

namespace RestifyServer.Dto;

public record UpdateAdmin(
    [param: MinLength(4)]
    string? Username,
    bool? WriteMode
);
