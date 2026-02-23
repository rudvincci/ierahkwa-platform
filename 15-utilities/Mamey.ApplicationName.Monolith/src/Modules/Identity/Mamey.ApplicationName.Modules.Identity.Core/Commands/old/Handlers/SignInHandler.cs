// // using Mamey.ApplicationName.Modules.Identity.Core.EF.Storage;
//
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Mamey.ApplicationName.Modules.Identity.Core.Events;
// using Mamey.ApplicationName.Modules.Identity.Core.Events.Rejected;
// using Mamey.ApplicationName.Modules.Identity.Core.Storage;
// using Mamey.Auth;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.CQRS.Commands;
// using Mamey.MicroMonolith.Abstractions.Messaging;
// using Mamey.MicroMonolith.Abstractions.Time;
// using Mamey.MicroMonolith.Infrastructure.Auth;
// using Mamey.Persistence.Redis;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Logging;
// using Microsoft.IdentityModel.Tokens;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Commands.Handlers;
//
// internal sealed class SignInHandler : ICommandHandler<SignIn>
// {
//     private ILogger<SignInHandler> _logger;
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly SignInManager<ApplicationUser> _signInManager;
//     private readonly IMessageBroker _messageBroker;
//     private readonly ICache _cache;
//     private readonly IRefreshTokenStore _refreshTokenStore;
//     private readonly AuthOptions _authOptions;
//     // private readonly IAuthManager _authManager;
//     private readonly string _issuer;
//     private readonly SigningCredentials _signingCredentials;
//
//     private readonly IClock _clock;
//     // private readonly IdentityUserStore _userStore;
//     private readonly IUserRequestStorage _requestStorage;
//     
//     public SignInHandler(ILogger<SignInHandler> logger, UserManager<ApplicationUser> userManager, 
//         SignInManager<ApplicationUser> signInManager, IMessageBroker messageBroker, ICache cache, 
//         IRefreshTokenStore refreshTokenStore,  IClock clock, /*ApplicationUserStore userStore,*/ IUserRequestStorage requestStorage, AuthOptions authOptions)
//     {
//         _logger = logger;
//         _userManager = userManager;
//         _signInManager = signInManager;
//         _messageBroker = messageBroker;
//         _cache = cache;
//         _refreshTokenStore = refreshTokenStore;
//         // _jwtOptions = jwtOptions;
//         _clock = clock;
//         // _userStore = userStore;
//         _requestStorage = requestStorage;
//         _authOptions = authOptions;
//         // _authManager = authManager;
//         _issuer = _authOptions.Issuer;
//         _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.IssuerSigningKey)),  SecurityAlgorithms.HmacSha256);
//     }
//
//     
//     public async Task HandleAsync(SignIn command, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByEmailAsync(command.Email);
//         if (user == null)
//         {
//             await _messageBroker.PublishAsync(new SignInUserRejected(user.Id, "Invalid credentials"), cancellationToken);
//             return;
//         }
//
//         if (!await _signInManager.CanSignInAsync(user))
//         {
//             await _messageBroker.PublishAsync(new SignInUserRejected(user.Id, "Invalid credentials"), cancellationToken);
//             return;
//         };
//         
//         // Attempt password sign-in
//         var result = await _signInManager.PasswordSignInAsync(user, command.Password, command.RememberMe, lockoutOnFailure: true);
//         if (result.RequiresTwoFactor)
//         {
//             // The user needs to perform 2FA. We do not issue tokens yet.
//             // Return a response indicating 2FA is required.
//             // return new JsonWebToken()
//             // {
//             //     AccessToken = null,
//             //     RefreshToken = null,
//             //     Expiry = null,
//             //     Email = user.Email,
//             //     UserId = user.Id,
//             //     Role = null,
//             //     RequiresTwoFactor = true
//             // };
//             
//         }
//         if (result.IsLockedOut)
//         {
//             await _messageBroker.PublishAsync(new SignInUserRejected(user.Id, "User is locked out"), cancellationToken);
//             return;
//         }
//
//         if (!result.Succeeded)
//         {
//             await _messageBroker.PublishAsync(new SignInUserRejected(user.Id, "Invalid credentials"), cancellationToken);
//             return;
//         }
//
//         // Sign in with cookie (if needed)
//         // await _signInManager.SignInAsync(user, request.RememberMe);
//         
//         // Issue JWT
//         var jwtToken = await IssueJwtTokenAsync(user, command.RememberMe);
//         var permissions = new Dictionary<string, IEnumerable<string>>();
//         
//         
//         _requestStorage.SetToken(user.Email, jwtToken);
//         
//         await _messageBroker.PublishAsync(new SignedIn(user.Id, _clock.CurrentDate()));
//         _logger.LogInformation($"User with ID: '{user.Id}' has signed in.");
//     }
//     public async Task<JsonWebToken> IssueJwtTokenAsync(ApplicationUser user, 
//         bool isPersistent,
//         IDictionary<string, string> claims = null,
//         CancellationToken cancellationToken = default)
//     {
//         // Issue JWT
//         var now = DateTime.UtcNow;
//         var issuerSigningKey = _authOptions.IssuerSigningKey;
//         if (issuerSigningKey is null)
//         {
//             throw new InvalidOperationException("Issuer signing key not set.");
//         }
//
//         if (_authOptions.Expiry is null)
//         {
//             throw new InvalidOperationException("Expiry signing key not set.");
//         }
//
//         // var userRoles = user.UserRoles.ToList();
//         var roles = await _userManager.GetRolesAsync(user);
//             
//         var expires = now.Add(_authOptions.Expiry.Value);
//         var jwtClaims = new List<Claim>
//         {
//             new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
//             new Claim(JwtRegisteredClaimNames.Email, user.Email),
//             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
//             new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeMilliseconds().ToString())
//         };
//         foreach (var role in roles)
//         {
//             jwtClaims.Add(new Claim(ClaimTypes.Role, role));
//         }
//         if (!string.IsNullOrWhiteSpace(_authOptions.Audience))
//         {
//             jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, _authOptions.Audience));
//         }
//         
//         if (claims?.Any() is true)
//         {
//             var customClaims = new List<Claim>();
//             foreach (var (claim, value) in claims)
//             {
//                 customClaims.Add(new Claim(claim, value));
//             }
//             jwtClaims.AddRange(customClaims);
//         }
//         
//         
//         var token = new JwtSecurityToken(
//             issuer: _issuer,
//             audience: _authOptions.Audience,
//             claims: jwtClaims,
//             notBefore: now,
//             expires: expires,
//             signingCredentials: _signingCredentials);
//         
//         var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
//
//         // Issue refresh token
//         var refreshToken = Guid.NewGuid().ToString("N");
//
//         if (_authOptions.RefreshTokenLifetime == null)
//         {
//             throw new InvalidOperationException("Refresh token lifetime not set.");
//         }
//         await _refreshTokenStore.SetTokenAsync(user.Id, refreshToken,
//             _authOptions.RefreshTokenLifetime.Value, cancellationToken);
//
//
//
//         return new JsonWebToken
//         {
//             AccessToken = accessToken,
//             Expires = new DateTimeOffset(expires).ToUnixTimeMilliseconds(),
//             Id = user.Id.ToString(),
//             Role = string.Join(",", roles) ?? string.Empty,
//             Claims = claims ?? new Dictionary<string, string>()
//         };
//     }
// }