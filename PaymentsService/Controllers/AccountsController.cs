using Microsoft.AspNetCore.Mvc;
using PaymentsService.Domain;
using PaymentsService.DTO;
using PaymentsService.Infrastructure;

namespace PaymentsService.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController : ControllerBase
{
    private readonly PaymentsDbContext _db;

    public AccountsController(PaymentsDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public IActionResult CreateAccount(CreateAccountRequest request)
    {
        if (_db.Accounts.Any(a => a.UserId == request.UserId))
            return BadRequest("Account already exists");

        _db.Accounts.Add(new Account
        {
            UserId = request.UserId,
            Balance = 0
        });

        _db.SaveChanges();
        return Ok();
    }

    [HttpPost("{userId}/topup")]
    public IActionResult TopUp(Guid userId, TopUpRequest request)
    {
        var account = _db.Accounts.Find(userId);
        if (account == null) return NotFound();

        account.Balance += request.Amount;
        _db.SaveChanges();

        return Ok();
    }

    [HttpGet("{userId}")]
    public IActionResult GetBalance(Guid userId)
    {
        var account = _db.Accounts.Find(userId);
        return account == null ? NotFound() : Ok(account.Balance);
    }
}