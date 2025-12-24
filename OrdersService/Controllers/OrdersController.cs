using Microsoft.AspNetCore.Mvc;
using OrdersService.Domain;
using OrdersService.Infrastructure;
using OrdersService.Messaging;

namespace OrdersService.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly OrdersDbContext _db;

    public OrdersController(OrdersDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse(request.UserId),
            Amount = request.Amount,
            Description = request.Description,
            Status = OrderStatus.NEW
        };

        _db.Orders.Add(order);

        _db.OutboxMessages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "OrderCreated",
            Payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                order.Id,
                order.UserId,
                order.Amount
            }),
            Processed = false
        });

        await _db.SaveChangesAsync();

        return Ok(order);
    }
}