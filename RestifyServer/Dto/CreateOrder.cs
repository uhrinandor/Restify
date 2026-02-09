using System.ComponentModel.DataAnnotations;
using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record CreateOrder(
    [param: Required]
    FindEntity Product,
    [param: Required]
    FindEntity Invoice
    );
