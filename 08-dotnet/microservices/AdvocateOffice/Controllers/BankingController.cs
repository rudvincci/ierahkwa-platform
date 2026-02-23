using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/banking")]
public class BankingController : ControllerBase
{
    private readonly DbService _db;

    public BankingController(DbService db) => _db = db;

    [HttpGet("accounts")]
    public IActionResult List() => Ok(_db.BankAccounts.OrderByDescending(a => a.CreatedAt));

    [HttpPost("accounts")]
    public IActionResult Create([FromBody] BankAccount body)
    {
        body.Id = _db.NextId("bankAccounts");
        body.CreatedAt = DateTime.UtcNow;
        _db.BankAccounts.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }

    [HttpPut("accounts/{id:int}")]
    public IActionResult Update(int id, [FromBody] BankAccount body)
    {
        var a = _db.BankAccounts.FirstOrDefault(x => x.Id == id);
        if (a == null) return NotFound();
        a.Name = body.Name;
        a.AccountNumber = body.AccountNumber;
        a.BankName = body.BankName;
        a.Balance = body.Balance;
        _db.SaveChanges();
        return Ok(a);
    }

    [HttpDelete("accounts/{id:int}")]
    public IActionResult Delete(int id)
    {
        var a = _db.BankAccounts.FirstOrDefault(x => x.Id == id);
        if (a == null) return NotFound();
        _db.BankAccounts.Remove(a);
        _db.SaveChanges();
        return NoContent();
    }
}
