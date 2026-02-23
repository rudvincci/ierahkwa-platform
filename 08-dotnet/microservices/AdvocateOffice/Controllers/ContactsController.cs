using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/contacts")]
public class ContactsController : ControllerBase
{
    private readonly DbService _db;

    public ContactsController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.Contacts.OrderByDescending(c => c.CreatedAt));

    [HttpPost]
    public IActionResult Create([FromBody] Contact body)
    {
        body.Id = _db.NextId("contacts");
        body.CreatedAt = DateTime.UtcNow;
        _db.Contacts.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }
}
