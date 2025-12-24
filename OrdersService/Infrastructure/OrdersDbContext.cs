using Microsoft.EntityFrameworkCore;
using OrdersService.Domain;
using OrdersService.Messaging;

namespace OrdersService.Infrastructure;

public class OrdersDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }
}