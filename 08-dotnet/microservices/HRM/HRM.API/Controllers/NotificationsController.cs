using HRM.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IHRMService _hrm;

    public NotificationsController(IHRMService hrm) => _hrm = hrm;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? employeeId, [FromQuery] bool? unreadOnly) =>
        Ok(await _hrm.GetNotificationsAsync(employeeId, unreadOnly));
}
