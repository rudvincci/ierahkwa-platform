using System.Security.Claims;
using AppBuilder.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoices;
    private readonly IAuthService _auth;

    public InvoicesController(IInvoiceService invoices, IAuthService auth)
    {
        _invoices = invoices;
        _auth = auth;
    }

    private string? UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public IActionResult List() => Ok(_invoices.GetByUser(UserId!));

    [HttpGet("{id}")]
    public IActionResult Get(string id)
    {
        var inv = _invoices.GetById(id);
        if (inv == null || inv.UserId != UserId) return NotFound(new { error = "Invoice not found" });
        return Ok(inv);
    }

    [HttpGet("{id}/html")]
    public IActionResult GetHtml(string id)
    {
        var inv = _invoices.GetById(id);
        if (inv == null || inv.UserId != UserId) return NotFound();
        var user = _auth.GetUserById(UserId!);
        return Content(_invoices.GetInvoiceHtml(inv, user), "text/html");
    }
}
