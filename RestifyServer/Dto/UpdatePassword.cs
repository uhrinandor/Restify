using System.ComponentModel.DataAnnotations;

namespace RestifyServer.Dto;

public record UpdatePassword(
    [param: Required, MinLength(6)] string OldPassword,
    [param: Required, MinLength(6)] string NewPassword
);
