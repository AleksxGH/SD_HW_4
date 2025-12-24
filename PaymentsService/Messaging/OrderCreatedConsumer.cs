using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using PaymentsService.Infrastructure;
using PaymentsService.Domain;

namespace PaymentsService.Messaging;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IModel _channel;

    public OrderCreatedConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.ExchangeDeclare("orders", ExchangeType.Direct);
        _channel.QueueDeclare("payments.order.created", true, false, false);
        _channel.QueueBind("payments.order.created", "orders", "order.created");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var doc = JsonDocument.Parse(json);

            var eventId = doc.RootElement.GetProperty("eventId").GetGuid();
            var orderId = doc.RootElement.GetProperty("orderId").GetGuid();
            var userId = doc.RootElement.GetProperty("userId").GetGuid();
            var amount = doc.RootElement.GetProperty("amount").GetInt32();

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();

            using var tx = db.Database.BeginTransaction();

            if (db.InboxMessages.Any(e => e.EventId == eventId))
                return;

            var account = db.Accounts.Find(userId);
            var success = account != null && account.Balance >= amount;

            if (success)
                account!.Balance -= amount;

            db.InboxMessages.Add(new InboxMessage { EventId = eventId });

            db.OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Payload = JsonSerializer.Serialize(new
                {
                    eventId,
                    orderId,
                    status = success ? "SUCCESS" : "FAILED"
                }),
                Processed = false
            });

            db.SaveChanges();
            tx.Commit();
        };

        _channel.BasicConsume("payments.order.created", true, consumer);
        return Task.CompletedTask;
    }
}
