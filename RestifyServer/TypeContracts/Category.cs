namespace RestifyServer.TypeContracts;

public class Category : Entity
{
    public string Name { get; set; } = string.Empty;
    public NestedCategory? Parent { get; set; }
    public List<NestedCategory> Children { get; set; } = new();
}

public class NestedCategory : Entity
{
    public string Name { get; set; } = string.Empty;
}
