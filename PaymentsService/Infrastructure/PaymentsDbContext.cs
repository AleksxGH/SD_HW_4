using Microsoft.EntityFrameworkCore;
using PaymentsService.Domain;
using PaymentsService.Messaging;

namespace PaymentsService.Infrastructure;

public class PaymentsDbContext : DbContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options) { }
}