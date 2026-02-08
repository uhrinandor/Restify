using System.ComponentModel.DataAnnotations;

namespace RestifyServer.Dto;

public record CreateTable(
    [param: Required] int Number
);
