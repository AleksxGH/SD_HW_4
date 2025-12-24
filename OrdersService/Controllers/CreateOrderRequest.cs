using System.ComponentModel.DataAnnotations;

namespace OrdersService.Controllers;

public class CreateOrderRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;
}