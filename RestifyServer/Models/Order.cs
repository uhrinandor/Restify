using RestifyServer.Models.Enums;

namespace RestifyServer.Models;

public class Order : Entity
{
    public OrderStatus Status { get; set; } = OrderStatus.New;
    
    public Product Product { get; set; } = null!;
    
    public Invoice Invoice { get; set; } = null!;
}
