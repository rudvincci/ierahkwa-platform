// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using Mamey.Casino.Domain.Entities;
// using Mamey.Casino.Infrastructure.Stores;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using Microsoft.IdentityModel.JsonWebTokens;
//
// using Mamey.Casino.Infrastructure.Auth;
// using Mamey.Time;
// using Microsoft.IdentityModel.Tokens;
// using JsonWebToken = Mamey.Casino.Infrastructure.Auth.JsonWebToken;
// using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
//
//
// namespace Mamey.Casino.Infrastructure.EF.Managers;
//
// public class ApplicationSignInManager : SignInManager<ApplicationUser>
// {
//     private static readonly Dictionary<string, IEnumerable<string>> EmptyClaims = new();
//     private readonly AuthOptions _authOptions;
//     private readonly IRefreshTokenStore _refreshTokenStore;
//     private readonly ApplicationUserManager _userManager;
//     private readonly ApplicationRoleManager _roleManager;
//     private readonly string _issuer;
//     private readonly SigningCredentials _signingCredentials;
//     public ApplicationSignInManager(
//         ApplicationUserManager userManager,
//         IHttpContextAccessor contextAccessor,
//         IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
//         IOptions<IdentityOptions> optionsAccessor,
//         ILogger<SignInManager<ApplicationUser>> logger,
//         IAuthenticationSchemeProvider schemes,
//         IUserConfirmation<ApplicationUser> confirmation, ApplicationRoleManager roleManager, IRefreshTokenStore refreshTokenStore, AuthOptions authOptions)
//         : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
//     {
//         _userManager = userManager;
//         _roleManager = roleManager;
//         _refreshTokenStore = refreshTokenStore;
//         _authOptions = authOptions;
//         _issuer = _authOptions.Issuer;
//         _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.IssuerSigningKey)),  SecurityAlgorithms.HmacSha256);
//     }
//
//     public async Task<JsonWebToken> IssueJwtTokenAsync(ApplicationUser user, 
//         bool isPersistent,
//         IDictionary<string, IEnumerable<string>> claims = null,
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
//         var roles = _roleManager.Roles
//             .Where(r => 
//                 user.UserRoles.Select(c=> c.RoleId)
//                     .Contains(r.Id)).Select(c=> c.Name);
//         
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
//             foreach (var (claim, values) in claims)
//             {
//                 customClaims.AddRange(values.Select(value => new Claim(claim, value)));
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
//             Expiry = new DateTimeOffset(expires).ToUnixTimeMilliseconds(),
//             UserId = user.Id,
//             Role = string.Join(",", roles) ?? string.Empty,
//             Claims = claims ?? EmptyClaims
//         };
//     }
//
//     public override Task SignInAsync(ApplicationUser user, bool isPersistent, string? authenticationMethod = null)
//     {
//         return Task.CompletedTask;// base.SignInAsync(user, isPersistent, authenticationMethod);
//     }
//
//     
//
//     
// }