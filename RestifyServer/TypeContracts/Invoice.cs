using RestifyServer.Models.Enums;

namespace RestifyServer.TypeContracts;

public class Invoice : Entity
{
    public DateTime? ClosedAt { get; set; } = null;

    public decimal Tip { get; set; } = 0;

    public Table Table { get; set; } = null!;

    public Waiter Waiter { get; set; } = null!;

    public List<Order> Orders { get; set; } = new();

    public PaymentType Payment { get; set; } = PaymentType.Card;
}
