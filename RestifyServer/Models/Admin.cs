using RestifyServer.Models.Enums;

namespace RestifyServer.Models;

public class Admin : Entity
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public Permission AccessLevel { get; set; } = Permission.Read;
}
