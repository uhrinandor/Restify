using System.ComponentModel.DataAnnotations;
using RestifyServer.TypeContracts;

namespace RestifyServer.Dto;

public record CreateInvoice(
    [param: Required]
    Waiter Waiter,

    [param: Required]
    Table Table
);
