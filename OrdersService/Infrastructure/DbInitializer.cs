namespace OrdersService.Infrastructure;

public static class DbInitializer
{
    public static void Init(OrdersDbContext db)
    {
        db.Database.EnsureCreated();
    }
}