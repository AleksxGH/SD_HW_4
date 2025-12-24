using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Domain;

public class Account
{
    [Key]
    public Guid UserId { get; set; }

    public int Balance { get; set; }
}