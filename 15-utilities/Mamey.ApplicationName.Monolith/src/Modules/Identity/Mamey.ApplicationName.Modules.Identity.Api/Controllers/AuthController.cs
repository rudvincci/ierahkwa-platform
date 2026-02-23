using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Identity.Api.Routes;
using Mamey.ApplicationName.Modules.Identity.Contracts.Commands;
using Mamey.ApplicationName.Modules.Identity.Contracts.Dto;
using Mamey.Auth.Identity;
using Mamey.Auth.Identity.Constants;
using Mamey.Auth.Identity.Entities;
using Mamey.Auth.Identity.Managers;
using Mamey.MicroMonolith.Abstractions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MameySignInResult = Mamey.Auth.Identity.Managers.MameySignInResult;

namespace Mamey.ApplicationName.Modules.Identity.Api.Controllers;

[AllowAnonymous]
internal class AuthController : BaseController
{
    private readonly MameySignInManager       _signInMgr;
    private readonly ITokenService        _tokenSvc;
    private readonly MameyClaimsPrincipalFactory _claimsFactory;
    private readonly MameyUserManager         _userMgr;
    private readonly IImpersonationContext _impCtx;

    public AuthController(MameySignInManager signInMgr, ITokenService tokenSvc, MameyClaimsPrincipalFactory claimsFactory, 
        MameyUserManager userMgr, IImpersonationContext impCtx)
    {
        _signInMgr = signInMgr;
        _tokenSvc = tokenSvc;
        _claimsFactory = claimsFactory;
        _userMgr = userMgr;
        _impCtx = impCtx;
    }
    /// <summary>
    /// Password login (may return MFA required).
    /// </summary>
    
    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> Login([FromBody] Login command)
    {
        var result = await _signInMgr.PasswordSignInAsync(
            command.Username.ToUpperInvariant(),
            command.Password,
            command.RememberMe,
            false,
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
            Request.Headers["User-Agent"]);

        // if (result == MameySignInResult.RequiresMfa())
        //     return Ok(LoginResult.MfaRequired);
        //
        // if (result != SignInResult.Success)
        //     return Unauthorized();

        // On success, issue tokens
        var user = await _userMgr.FindByIdAsync(
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        var (access, refresh) = await _tokenSvc.CreateTokensAsync(user!, HttpContext.RequestAborted);

        return Ok(new LoginResultDto
        {
            Succeeded = true,
            IsLockedOut = false,
            RequiresTwoFactor = false,
            RedirectUrl = command.ReturnUrl
        });
    }

//     /// <summary>
//     /// Verify MFA and issue tokens.
//     /// </summary>
//     [HttpPost("verify-mfa", Name = RouteNames.VerifyMfa)]
//     public async Task<ActionResult<JwtResponse>> VerifyMfa([FromBody] MFAChallengeRequest req)
//     {
//         var result = await _signInMgr.VerifyMfaAsync(
//             req.UserId, req.Code,
//             HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
//             Request.Headers["User-Agent"]);
//
//         if (result != SignInResult.Success)
//             return Unauthorized();
//
//         var user = await _userMgr.FindByIdAsync(req.UserId);
//         var (access, refresh) = await _tokenSvc.CreateTokensAsync(user!, HttpContext.RequestAborted);
//
//         return Ok(new JwtResponse
//         {
//             AccessToken  = access,
//             RefreshToken = refresh,
//             ExpiresAt    = DateTime.UtcNow.AddMinutes(60)
//         });
//     }
//
//     /// <summary>
//     /// Exchange a valid refresh token for new tokens.
// /// </summary>
//     [HttpPost("refresh", Name = RouteNames.Refresh)]
//     public async Task<ActionResult<JwtResponse>> Refresh([FromBody] RefreshRequest req)
//     {
//         // TODO: validate refresh via revocation store, then re‑issue
//         return BadRequest();
//     }
//
//     /// <summary>
//     /// Register a new user under current tenant.
// /// </summary>
//     [HttpPost("register", Name = RouteNames.Register)]
//     public async Task<ActionResult> Register([FromBody] RegisterRequest req)
//     {
//         var user = new ApplicationUser
//         {
//             UserName = req.UserName,
//             NormalizedUserName = req.UserName.ToUpperInvariant(),
//             Email = req.Email,
//             NormalizedEmail = req.Email.ToUpperInvariant(),
//             FullName = req.FullName,
//             PasswordHash = req.Password // will be hashed inside manager
//         };
//
//         await _userMgr.CreateAsync(user, User.FindFirstValue(ClaimTypes.NameIdentifier)!);
//         return CreatedAtRoute(RouteNames.Login, null);
//     }
//
//     /// <summary>
//     /// Change current user’s password.
//     /// </summary>
//     [HttpPost("change-password", Name = RouteNames.ChangePassword)]
//     public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
//     {
//         // TODO: implement via UserManager + PasswordValidator
//         return NoContent();
//     }
//
//     /// <summary>
//     /// Sign out current user.
//     /// </summary>
//     [HttpPost("logout", Name = RouteNames.Logout)]
//     public async Task<ActionResult> Logout()
//     {
//         var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
//         await _signInMgr.SignOutAsync(userId, HttpContext.RequestAborted);
//         return NoContent();
//     }
}