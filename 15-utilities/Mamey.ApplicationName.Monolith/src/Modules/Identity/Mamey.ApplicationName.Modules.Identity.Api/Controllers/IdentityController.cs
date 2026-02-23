// using Mamey.ApplicationName.Modules.Identity.Core.Storage;
// using Mamey.MicroMonolith.Abstractions.Contexts;
// using Mamey.MicroMonolith.Abstractions.Dispatchers;
// using Microsoft.AspNetCore.Http;
//
// namespace Mamey.ApplicationName.Modules.Identity.Api.Controllers
// {
//     internal class IdentityController : BaseController
//     {
//         private readonly IDispatcher _dispatcher;
//         private readonly IContext _context;
//         private readonly IHttpContextAccessor _httpContextAccessor;
//         private readonly IUserRequestStorage _userRequestStorage;
//         private readonly CookieOptions _cookieOptions;
//
//         public IdentityController(IContext context, IDispatcher dispatcher, IHttpContextAccessor httpContextAccessor, CookieOptions cookieOptions, IUserRequestStorage userRequestStorage)
//         {
//             _context = context;
//             _dispatcher = dispatcher;
//             _httpContextAccessor = httpContextAccessor;
//             _cookieOptions = cookieOptions;
//             _userRequestStorage = userRequestStorage;
//         }
//         
//         
//
//         
//         
//     }
// }