using HRM.Core.Interfaces;
using HRM.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaxController : ControllerBase
{
    private readonly IHRMService _hrm;

    public TaxController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _hrm.GetTaxSetupsAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaxSetup t) => Ok(await _hrm.CreateTaxSetupAsync(t));
}
