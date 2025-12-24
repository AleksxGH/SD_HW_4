using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Messaging;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; }

    public string Payload { get; set; } = string.Empty;

    public bool Processed { get; set; }
}