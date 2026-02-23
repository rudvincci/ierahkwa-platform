using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly DbService _db;

    public InvoicesController(DbService db) => _db = db;

    [HttpGet]
    public IActionResult List() => Ok(_db.Invoices.OrderByDescending(i => i.CreatedAt));

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var inv = _db.Invoices.FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        var items = _db.InvoiceItems.Where(x => x.InvoiceId == id).ToList();
        return Ok(new { invoice = inv, items, clientName = _db.Clients.FirstOrDefault(c => c.Id == inv.ClientId)?.Name });
    }

    [HttpPost]
    public IActionResult Create([FromBody] InvoiceCreateRequest body)
    {
        var inv = new Invoice
        {
            Id = _db.NextId("invoices"),
            InvoiceNumber = body.InvoiceNumber ?? $"INV-{DateTime.UtcNow:yyyyMMdd}-{_db.Invoices.Count + 1}",
            ClientId = body.ClientId,
            CaseId = body.CaseId,
            IssueDate = body.IssueDate,
            DueDate = body.DueDate,
            SubTotal = body.SubTotal,
            TaxTotal = body.TaxTotal,
            Total = body.Total,
            Status = body.Status ?? "draft",
            CreatedAt = DateTime.UtcNow
        };
        _db.Invoices.Add(inv);
        _db.SaveChanges();
        return Ok(inv);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Invoice body)
    {
        var inv = _db.Invoices.FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        inv.InvoiceNumber = body.InvoiceNumber;
        inv.ClientId = body.ClientId;
        inv.CaseId = body.CaseId;
        inv.IssueDate = body.IssueDate;
        inv.DueDate = body.DueDate;
        inv.SubTotal = body.SubTotal;
        inv.TaxTotal = body.TaxTotal;
        inv.Total = body.Total;
        inv.Status = body.Status;
        _db.SaveChanges();
        return Ok(inv);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var inv = _db.Invoices.FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        _db.Invoices.Remove(inv);
        foreach (var i in _db.InvoiceItems.Where(x => x.InvoiceId == id).ToList())
            _db.InvoiceItems.Remove(i);
        _db.SaveChanges();
        return NoContent();
    }
}

public class InvoiceCreateRequest
{
    public string? InvoiceNumber { get; set; }
    public int ClientId { get; set; }
    public int? CaseId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal Total { get; set; }
    public string? Status { get; set; }
}
