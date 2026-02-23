using Microsoft.AspNetCore.Mvc;

namespace Mamey.Government.Modules.Passports.Api.Controllers;

[ApiController]
[Route(PassportsModule.BasePath)]
internal abstract class BaseController : ControllerBase
{
    protected ActionResult<T> OkOrNotFound<T>(T? model)
    {
        if (model is not null)
        {
            return Ok(model);
        }
        return NotFound();
    }
}
