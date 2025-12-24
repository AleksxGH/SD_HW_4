using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OrdersService.Domain;
using OrdersService.Infrastructure;

namespace OrdersService.Messaging;

public class PaymentResultConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection _connection;
    private IModel _channel;

    public PaymentResultConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;

        var factory = new ConnectionFactory { HostName = "rabbitmq" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare("payments", ExchangeType.Direct);
        _channel.QueueDeclare("orders.payment.result", true, false, false);
        _channel.QueueBind("orders.payment.result", "payments", "payment.result");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var doc = JsonDocument.Parse(json);
            var orderId = doc.RootElement.GetProperty("orderId").GetGuid();
            var status = doc.RootElement.GetProperty("status").GetString();

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

            var order = db.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status == "SUCCESS"
                    ? OrderStatus.FINISHED
                    : OrderStatus.CANCELED;
                db.SaveChanges();
            }
        };

        _channel.BasicConsume("orders.payment.result", true, consumer);
        return Task.CompletedTask;
    }
}