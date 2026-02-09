using System.ComponentModel.DataAnnotations;
using RestifyServer.Models.Enums;

namespace RestifyServer.Dto;

public record CloseInvoice(
    [param: Required]
    decimal Tip,
    [param: Required]
    PaymentType? Payment
    );
