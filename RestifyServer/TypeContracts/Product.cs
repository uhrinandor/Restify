namespace RestifyServer.TypeContracts;

public class Product : Entity
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; } = 0;

    public NestedCategory Category { get; set; } = null!;
}
