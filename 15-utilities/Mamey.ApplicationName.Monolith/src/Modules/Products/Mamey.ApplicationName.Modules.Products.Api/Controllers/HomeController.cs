using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Mamey.ApplicationName.Modules.Products.Api.Controllers
{
    [Route(ProductsModule.BasePath)]
    internal class HomeController : BaseController
    {
        [HttpGet]
        [SwaggerOperation(
            Summary = "Banking Product API Module Health",
            Description = "Fetches the health of the banking product's module",
            OperationId = "GetBankingProductsModuleHealth",
            Tags = new[] { "Health" }
        )]
        public ActionResult<string> Get() => "Products API";
    }
}