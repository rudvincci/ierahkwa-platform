using Microsoft.AspNetCore.Mvc;

namespace Mamey.ApplicationName.Modules.Notifications.Api.Controllers
{
    [ApiController]
    [Route(NotificationsModule.BasePath + "/[controller]")]
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