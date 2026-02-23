// using System;
// using System.Threading.Tasks;
// using Mamey.ApplicationName.Modules.Identity.Core.Commands;
// using Mamey.ApplicationName.Modules.Identity.Core.DTO;
// using Mamey.ApplicationName.Modules.Identity.Core.Queries;
// using Mamey.ApplicationName.Modules.Identity.Core.Storage;
// using Mamey.MicroMonolith.Abstractions.Contexts;
// using Mamey.MicroMonolith.Abstractions.Dispatchers;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace Mamey.ApplicationName.Modules.Identity.Api.Controllers
// {
//     [Route(IdentityModule.BasePath)]
//     internal class HomeController : BaseController
//     {
//         private const string AccessTokenCookie = "__access-token";
//         private readonly IDispatcher _dispatcher;
//         private readonly IContext _context;
//         private readonly IHttpContextAccessor _httpContextAccessor;
//         private readonly IUserRequestStorage _userRequestStorage;
//         private readonly CookieOptions _cookieOptions;
//         
//         public HomeController(IDispatcher dispatcher, IContext context, IHttpContextAccessor httpContextAccessor, IUserRequestStorage userRequestStorage, CookieOptions cookieOptions)
//         {
//             _dispatcher = dispatcher;
//             _context = context;
//             _httpContextAccessor = httpContextAccessor;
//             _userRequestStorage = userRequestStorage;
//             _cookieOptions = cookieOptions;
//         }
// // #if DEBUG
//         [HttpGet]
//         public ActionResult<string> Get() => "Identity API";
//         
//         
//         [HttpPost("sign-up")]
//         [ApiExplorerSettings(IgnoreApi = false)]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         public async Task<ActionResult> SignUpAsync(CreateUser command)
//         {
//             await _dispatcher.SendAsync(command);
//             return NoContent();
//         }
// // #endif
//         [HttpPost("sign-in")]
//         [SwaggerOperation(
//             Summary = "Sign in",
//             Description = "Sign into account.",
//             OperationId = "SignInAsync",
//             Tags = new[] { "Identity" }
//         )]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         public async Task<ActionResult<ApplicationUserDto>> SignInAsync(SignIn command)
//         {
//             await _dispatcher.SendAsync(command);
//             var jwt = _userRequestStorage.GetToken(command.Email);
//             var user = await _dispatcher.QueryAsync(new GetUserById(jwt.UserId));
//             var payload = new { jwt = jwt, user = user };
//             AddCookie(AccessTokenCookie, jwt.AccessToken);
//             return Ok(payload);
//         }
//
//         [HttpGet("me")]
//         [Authorize]
//         [SwaggerOperation(
//             Summary = "My Account",
//             Description = "Gets current user account",
//             OperationId = "GetMeAsync",
//             Tags = new[] { "Accounts" }
//         )]
//         [ProducesResponseType(StatusCodes.Status200OK)]
//         [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//         public async Task<ActionResult<ApplicationUserDto?>> GetMeAsync()
//         {
//             var user = await _dispatcher.QueryAsync(new GetUserById(_context.Identity.Id));
//             return OkOrNotFound(user);
//         }
//
//         // [HttpGet]
//         // public async Task<ActionResult<PagedResult<ApplicationUserDto>>> Get(BrowseUsers query) 
//         //     => await _dispatcher.QueryAsync(query);
//         
//         [Authorize]
//         [HttpDelete("sign-out")]
//         [SwaggerOperation(
//             Summary = "Sign out",
//             Description = "Sign out of account.",
//             OperationId = "SignOutAsync",
//             Tags = new[] { "Identity" }
//         )]
//         [ProducesResponseType(StatusCodes.Status204NoContent)]
//         [ProducesResponseType(StatusCodes.Status400BadRequest)]
//         [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//         public async Task<ActionResult> SignOutAsync()
//         {
//             await _dispatcher.SendAsync(new SignOut(_context.Identity.Id));
//             DeleteCookie(AccessTokenCookie);
//             return NoContent();
//         }
//         private void AddCookie(string key, string value) => Response.Cookies.Append(key, value, _cookieOptions);
//
//         private void DeleteCookie(string key) => Response.Cookies.Delete(key, _cookieOptions);
//         
//     }
// }