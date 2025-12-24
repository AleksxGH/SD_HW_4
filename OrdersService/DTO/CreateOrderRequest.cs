namespace OrdersService.DTO;

public class CreateOrderRequest
{
    public string UserId { get; set; } 
    public int Amount { get; set; }
    public string Description { get; set; }
}