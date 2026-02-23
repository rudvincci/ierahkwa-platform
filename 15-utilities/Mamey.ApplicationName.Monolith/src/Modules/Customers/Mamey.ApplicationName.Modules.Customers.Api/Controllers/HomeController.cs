using Microsoft.AspNetCore.Mvc;

namespace Mamey.ApplicationName.Modules.Customers.Api.Controllers
{
    [Route(CustomersModule.BasePath)]
    internal class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult<string> Get() => "Customers API";
    }
}