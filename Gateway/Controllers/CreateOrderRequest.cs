namespace Gateway.Controllers;

public class CreateOrderRequest
{
    public string UserId { get; set; } = default!;
    public int Amount { get; set; }
    public string Description { get; set; } = default!;
}