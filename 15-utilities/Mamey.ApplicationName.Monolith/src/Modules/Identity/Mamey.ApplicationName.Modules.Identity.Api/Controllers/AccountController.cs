// using System;
// using System.Net;
// using System.Threading.Tasks;
// using Mamey.ApplicationName.Modules.Identity.Core.Commands;
// using Mamey.ApplicationName.Modules.Identity.Core.DTO;
// using Mamey.ApplicationName.Modules.Identity.Core.Queries;
// using Mamey.ApplicationName.Modules.Identity.Core.Services;
// using Mamey.CQRS.Queries;
// using Mamey.MicroMonolith.Abstractions.Contexts;
// using Mamey.MicroMonolith.Abstractions.Dispatchers;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace Mamey.ApplicationName.Modules.Identity.Api.Controllers;
// internal class AccountController : BaseController
// {
//     private readonly IDispatcher _dispatcher;
//     private readonly IContext _context;
//     private readonly IIdentityService _identityService;
//     
//     public AccountController(IDispatcher dispatcher, IContext context, IIdentityService identityService)
//     {
//         _dispatcher = dispatcher;
//         _context = context;
//         _identityService = identityService;
//     }
//
//     [HttpGet]
//     [Authorize]
//     [SwaggerOperation(
//         Summary = "Get account",
//         Description = "Gets user account",
//         OperationId = "GetUserAccounts",
//         Tags = new[] { "Accounts" }
//     )]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//     public async Task<ActionResult<PagedResult<ApplicationUserDto>>> GetAsync()
//     {
//         var users = await _dispatcher.QueryAsync(new BrowseUsers());
//         return OkOrNotFound(users);
//     }
//
//     [HttpGet("{id:guid}")]
//     [Authorize]
//     [SwaggerOperation(
//         Summary = "Get account",
//         Description = "Gets user account",
//         OperationId = "GetUserAccounts",
//         Tags = new[] { "Accounts" }
//     )]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//     public async Task<ActionResult<ApplicationUserDto?>> GetByIdAsync(Guid id)
//     {
//         var user = await _dispatcher.QueryAsync(new GetUserById(id));
//         return OkOrNotFound(user);
//     }
//     
//     [HttpPost("confirm-email")]
//     [Authorize]
//     [SwaggerOperation(
//         Summary = "Confirm email",
//         Description = "Confirms user account",
//         OperationId = "GetUserAccounts",
//         Tags = new[] { "Accounts" }
//     )]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//     public async Task<ActionResult<ApplicationUserDto?>> ConfirmEmailAsync(ConfirmEmail command)
//     {
//         var result = await _identityService.ConfirmEmailAsync(command.UserId, command.Token);
//         return result.Item1 switch
//         {
//             HttpStatusCode.Accepted => Accepted(result),
//             HttpStatusCode.NotFound => NotFound(result.Item3),
//             HttpStatusCode.BadRequest => BadRequest(result.Item3),
//             _ => BadRequest()
//         };
//         // await _dispatcher.SendAsync(command);
//         //return Accepted();
//     }
//     
//     [HttpPost("password-reset/initiate")]
//     [AllowAnonymous]
//     [SwaggerOperation(
//         Summary = "Initiate password reset",
//         Description = "Initiates a password reset request on a user account.",
//         OperationId = "GetUserAccounts",
//         Tags = new[] { "Accounts" }
//     )]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//     public async Task<ActionResult<ApplicationUserDto?>> ConfirmEmailAsync(InitiatePasswordReset command)
//     {
//         await _dispatcher.SendAsync(command);
//         return Accepted();
//     }
// }