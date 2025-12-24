using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using OrdersService.Infrastructure;

namespace OrdersService.Messaging;

public class OutboxPublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnection _connection;

    public OutboxPublisher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        var factory = new ConnectionFactory
        {
            HostName = "rabbitmq"
        };
        _connection = factory.CreateConnection();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var channel = _connection.CreateModel();
        channel.ExchangeDeclare("orders", ExchangeType.Direct);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var messages = db.OutboxMessages
                .Where(x => !x.Processed)
                .ToList();

            foreach (var msg in messages)
            {
                var body = Encoding.UTF8.GetBytes(msg.Payload);
                channel.BasicPublish("orders", "order.created", null, body);
                msg.Processed = true;
            }

            await db.SaveChangesAsync();
            await Task.Delay(1000, stoppingToken);
        }
    }
}