// using Microsoft.AspNetCore.Authentication;
// using Microsoft.Identity.Client;
// using Microsoft.Extensions.Options;
// using System.Security.Claims;
// using System.Text.Encodings.Web;
// using Microsoft.Extensions.Logging;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.JwtBearer;

// namespace Mamey.Auth.Azure.B2B;

// public class AzureB2BAuthenticationHandler : AuthenticationHandler<AzureB2BOptions>
// {
//     private readonly IConfidentialClientApplication _msalClient;
//     private readonly AzureB2BOptions _options;
//     public AzureB2BAuthenticationHandler(IOptionsMonitor<AzureB2BOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
//         : base(options, logger, encoder, clock)
//     {
//         _msalClient = ConfidentialClientApplicationBuilder.Create(options.CurrentValue.ClientId)
//             .WithClientSecret(options.CurrentValue.ClientSecret)
//             .WithAuthority(options.CurrentValue.Authority)
//             .Build();
//     }

//     protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//     {
//         try
//         {
//             if (!Request.Headers.ContainsKey("Authorization"))
//             {
//                 return AuthenticateResult.Fail("Authorization header missing.");
//             }

//             var token = Request.Headers["Authorization"].ToString().Split(" ").Last();
//             if (string.IsNullOrEmpty(token))
//             {
//                 return AuthenticateResult.Fail("Token missing.");
//             }

//             var result = await _msalClient.AcquireTokenOnBehalfOf(Options.Scopes, new UserAssertion(token))
//                                             .ExecuteAsync();

//             if (result != null)
//             {
//                 var claims = new List<Claim>
//                 {
//                     // new Claim(ClaimTypes.Name, result.Account.Username),
//                     // new Claim(ClaimTypes.NameIdentifier, result.Account.HomeAccountId.Identifier)
//                 };

//                 var identity = new ClaimsIdentity(claims, Scheme.Name);
//                 var principal = new ClaimsPrincipal(identity);

//                 return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
//             }

//             return AuthenticateResult.Fail("Failed to acquire token.");
//         }
//         catch (MsalUiRequiredException ex)
//         {
//             return AuthenticateResult.Fail($"MSAL UI required: {ex.Message}");
//         }
//         catch (MsalServiceException ex)
//         {
//             return AuthenticateResult.Fail($"MSAL service exception: {ex.Message}");
//         }
//         catch (Exception ex)
//         {
//             return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
//         }
//     }
// }
