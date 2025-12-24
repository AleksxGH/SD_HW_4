using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Messaging;

public class InboxMessage
{
    [Key]
    public Guid EventId { get; set; }
}