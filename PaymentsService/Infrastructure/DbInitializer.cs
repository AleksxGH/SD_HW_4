namespace PaymentsService.Infrastructure;

public static class DbInitializer
{
    public static void Init(PaymentsDbContext db)
    {
        db.Database.EnsureCreated();
    }
}