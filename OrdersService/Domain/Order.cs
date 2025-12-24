using System.ComponentModel.DataAnnotations;

namespace OrdersService.Domain;

public class Order
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int Amount { get; set; }

    public string Description { get; set; } = string.Empty;

    public OrderStatus Status { get; set; }
}