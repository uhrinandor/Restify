namespace RestifyServer.Models;

public class Category : Entity
{
    public string Name { get; set; } = string.Empty;

    public Category? Parent { get; set; }

    public List<Category> Children { get; set; } = new();

    public List<Product> Products { get; set; } = new();
}
