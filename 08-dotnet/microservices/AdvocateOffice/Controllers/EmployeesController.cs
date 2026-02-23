using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController : ControllerBase
{
    private readonly DbService _db;

    public EmployeesController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.Employees.OrderByDescending(e => e.CreatedAt));

    [HttpPost]
    public IActionResult Create([FromBody] Employee body)
    {
        body.Id = _db.NextId("employees");
        body.CreatedAt = DateTime.UtcNow;
        _db.Employees.Add(body);
        _db.SaveChanges();
        return Ok(body);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Employee body)
    {
        var e = _db.Employees.FirstOrDefault(x => x.Id == id);
        if (e == null) return NotFound();
        e.Name = body.Name;
        e.Email = body.Email;
        e.Phone = body.Phone;
        e.Designation = body.Designation;
        e.DateOfBirth = body.DateOfBirth;
        _db.SaveChanges();
        return Ok(e);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var e = _db.Employees.FirstOrDefault(x => x.Id == id);
        if (e == null) return NotFound();
        _db.Employees.Remove(e);
        _db.SaveChanges();
        return NoContent();
    }
}
