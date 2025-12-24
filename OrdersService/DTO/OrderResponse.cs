using OrdersService.Domain;

namespace OrdersService.DTO;

public class OrderResponse
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public int Amount { get; set; }
}