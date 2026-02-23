using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DbService _db;

    public DashboardController(DbService db) => _db = db;

    [HttpGet("stats")]
    public IActionResult Stats()
    {
        return Ok(new
        {
            clients = _db.Clients.Count,
            cases = _db.Cases.Count,
            tasks = _db.Tasks.Count(ct => ct.Status != "done"),
            invoices = _db.Invoices.Count,
            invoicesPaid = _db.Invoices.Count(i => i.Status == "paid")
        });
    }
}
