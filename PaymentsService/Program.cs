using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure;
using PaymentsService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<RabbitMqInitializer>();

builder.Services.AddDbContext<PaymentsDbContext>(o =>
    o.UseSqlite("Data Source=payments.db"));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddHostedService<OrderCreatedConsumer>();
builder.Services.AddHostedService<OutboxPublisher>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DbInitializer.Init(scope.ServiceProvider.GetRequiredService<PaymentsDbContext>());
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();