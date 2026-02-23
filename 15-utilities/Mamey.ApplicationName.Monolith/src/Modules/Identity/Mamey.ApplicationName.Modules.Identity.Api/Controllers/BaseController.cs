using Microsoft.AspNetCore.Mvc;

namespace Mamey.ApplicationName.Modules.Identity.Api.Controllers
{
    [ApiController]
    [Route(IdentityModule.BasePath + "/[controller]")]
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
}