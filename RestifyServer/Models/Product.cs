namespace RestifyServer.Models;

public class Product : Entity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; } = 0;

    public Category Category { get; set; } = null!;
}
