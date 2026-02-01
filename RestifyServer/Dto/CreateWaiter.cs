using System.ComponentModel.DataAnnotations;

namespace RestifyServer.Dto;

public record CreateWaiter(
    [param: Required, MinLength(4)]
    string Name,
    [param: Required, MinLength(4)]
    string Username,
    [param: Required, MinLength(6)]
    string Password
    );
