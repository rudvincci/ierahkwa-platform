using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.ApplicationName.Modules.Notifications.Api.Controllers
{
    [Route(NotificationsModule.BasePath)]
    internal class HomeController : BaseController
    {
        [HttpGet]
        [SwaggerOperation(
            Summary = "Notifications API Module Health",
            Description = "Fetches the health of the notification's module",
            OperationId = "GetNotificationsModuleHealth",
            Tags = new[] { "Health" }
        )]
        public ActionResult<string> Get() => "Notifications API";
    }
}