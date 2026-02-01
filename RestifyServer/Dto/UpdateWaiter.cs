using System.ComponentModel.DataAnnotations;

namespace RestifyServer.Dto;

public record UpdateWaiter(
    [param: MinLength(4)]
    string Username,
    [param: MinLength(4)]
    string Name
);
