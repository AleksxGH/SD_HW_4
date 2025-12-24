using Microsoft.EntityFrameworkCore;
using OrdersService.Infrastructure;
using OrdersService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<RabbitMqInitializer>();

builder.Services.AddDbContext<OrdersDbContext>(o =>
    o.UseSqlite("Data Source=orders.db"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<OutboxPublisher>();
builder.Services.AddHostedService<PaymentResultConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DbInitializer.Init(
        scope.ServiceProvider.GetRequiredService<OrdersDbContext>()
    );
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();