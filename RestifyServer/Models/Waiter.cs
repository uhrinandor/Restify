namespace RestifyServer.Models;

public class Waiter : Entity
{
    public string Name { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public List<Invoice> Invoices { get; set; } = new();
}
