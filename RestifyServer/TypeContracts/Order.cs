using RestifyServer.Models.Enums;

namespace RestifyServer.TypeContracts;

public class Order : Entity
{
    public OrderStatus Status { get; set; } = OrderStatus.New;

    public Product Product { get; set; } = null!;

    public NestedInvoice Invoice { get; set; } = null!;
}
