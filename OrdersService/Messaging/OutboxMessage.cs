using System.ComponentModel.DataAnnotations;

namespace OrdersService.Messaging;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public bool Processed { get; set; }
}