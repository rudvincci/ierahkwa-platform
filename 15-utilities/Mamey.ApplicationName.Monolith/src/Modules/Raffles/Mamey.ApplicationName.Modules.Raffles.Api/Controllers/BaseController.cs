using Microsoft.AspNetCore.Mvc;
namespace Mamey.ApplicationName.Modules.Raffles.Api.Controllers;

[ApiController]
[Route(RafflesModule.BasePath + "/api/[controller]")]
internal abstract class BaseController : ControllerBase
{
    protected ActionResult<T> OkOrNotFound<T>(T model)
    {
        if (model is not null)
        {
            return Ok(model);
        }

        return NotFound();
    }
}