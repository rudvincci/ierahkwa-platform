using Microsoft.AspNetCore.Mvc;

namespace Mamey.Government.Modules.TravelIdentities.Api.Controllers;

[ApiController]
[Route(TravelIdentitiesModule.BasePath + "/[controller]")]
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
