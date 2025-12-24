using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace OrdersService.Infrastructure;

public class RabbitMqInitializer : BackgroundService
{
    private readonly IServiceProvider _provider;

    public RabbitMqInitializer(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var host = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "localhost";

        var factory = new ConnectionFactory
        {
            HostName = host,
            AutomaticRecoveryEnabled = true
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                Console.WriteLine("RabbitMQ is ready");
                break;
            }
            catch
            {
                Console.WriteLine("Waiting for RabbitMQ...");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}