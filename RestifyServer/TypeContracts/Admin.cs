using RestifyServer.Models.Enums;

namespace RestifyServer.TypeContracts;

public class Admin : Entity
{
    public string Username { get; set; } = string.Empty;

    public Permission AccessLevel { get; set; } = Permission.Read;
}
