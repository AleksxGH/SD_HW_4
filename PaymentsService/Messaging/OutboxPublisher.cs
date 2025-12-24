using RabbitMQ.Client;
using System.Text;
using PaymentsService.Infrastructure;

namespace PaymentsService.Messaging;

public class OutboxPublisher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IModel _channel;

    public OutboxPublisher(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        var factory = new ConnectionFactory { HostName = "localhost" };
        _channel = factory.CreateConnection().CreateModel();
        _channel.ExchangeDeclare("payments", ExchangeType.Direct);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();

            var messages = db.OutboxMessages.Where(m => !m.Processed).ToList();

            foreach (var msg in messages)
            {
                _channel.BasicPublish(
                    "payments",
                    "payment.result",
                    null,
                    Encoding.UTF8.GetBytes(msg.Payload)
                );
                msg.Processed = true;
            }

            await db.SaveChangesAsync();
            await Task.Delay(1000, stoppingToken);
        }
    }
}