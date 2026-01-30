using RestifyServer.Models.Enums;

namespace RestifyServer.Models;

public class Invoice : Entity
{
    public DateTime? ClosedAt = null;

    public decimal Tip = 0;

    public Table Table = null!;

    public Waiter Waiter = null!;

    public List<Order> Orders = new();

    public PaymentType Payment = PaymentType.Card;
}
