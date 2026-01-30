using System.ComponentModel.DataAnnotations;

namespace RestifyServer.Dto;

public record CreateAdmin(
    [param: Required, MinLength(4)]
    string Username,
    bool WriteMode,
    [param: Required, MinLength(6)]
    string Password
    );
