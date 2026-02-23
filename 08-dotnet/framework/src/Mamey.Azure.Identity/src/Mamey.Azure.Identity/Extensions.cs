//using System.Security.Claims;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using Mamey;
//using Mamey.Auth;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authentication.OpenIdConnect;
//using Microsoft.AspNetCore.Authorization.Policy;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Graph.Models.ExternalConnectors;
//using Microsoft.Identity.Web;
//using Microsoft.IdentityModel.Protocols.OpenIdConnect;
//using Microsoft.IdentityModel.Tokens;

//namespace Mamey.Azure.Identity;

//public static class Extensions
//{
//    public static WebApplicationBuilder AddJwtBearer(this WebApplicationBuilder builder, IConfiguration configuration)
//    {
//        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//            .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"))
//                .EnableTokenAcquisitionToCallDownstreamApi()
//                    .AddMicrosoftGraph(configuration.GetSection("DownstreamApi"))
//                    .AddInMemoryTokenCaches();
//        return builder;
//    }
//    public static WebApplicationBuilder AddOpenIdConnectWithCookies(this WebApplicationBuilder builder, IConfiguration configuration)
//    {
//        var azureAdOptions = builder.Services.GetOptions<AzureAdOptions>("AzureAd");
//        var authenticationBuilder = builder.Services.AddAuthentication(options =>
//        {
//            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//        });

//        authenticationBuilder

//            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
//            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
//            {
//                var azureAd = configuration.GetSection("AzureAd");
//                configuration.Bind("AzureAd", options);
//                options.Authority = azureAdOptions.Authority;
//                options.ClientId = azureAdOptions.ClientId;
//                options.ClientSecret = azureAdOptions.ClientSecret;
//                options.CallbackPath = azureAdOptions.CallbackPath;

//                options.RemoteAuthenticationTimeout = TimeSpan.FromSeconds(120);
//                options.ResponseType = OpenIdConnectResponseType.Code;
//                //options.ResponseType = "code";
//                options.GetClaimsFromUserInfoEndpoint = true;
//                options.SaveTokens = true;
//                options.UseTokenLifetime = true;
//                //options.SignInScheme = IdentityConstants.ApplicationScheme;
//                //options.Scope.Add("openid");f
//                //options.Scope.Add("profile");
//                //options.Scope.Add("email");
//                //options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");

//                options.MetadataAddress = $"https://login.microsoftonline.com/{azureAdOptions.TenantId}/.well-known/openid-configuration";
//                //options.Validate();
//            });
//        authenticationBuilder.AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"))
//                .EnableTokenAcquisitionToCallDownstreamApi()
//                    .AddMicrosoftGraph(configuration.GetSection("DownstreamApi"))
//                    .AddInMemoryTokenCaches();
//        //services.AddAuthorization(authorization =>
//        //{
//        //    foreach (var policy in policies)
//        //    {
//        //        authorization.AddPolicy(policy, x => x.RequireClaim("permissions", policy));
//        //    }
//        //});

//        //services.AddSingleton(CreateGraphServiceClient(configuration));

//        return builder;
//    }

//    public static IServiceCollection RegisterAzureApplication(this IServiceCollection services)
//    {
//        var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
//        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//            .AddMicrosoftIdentityWebApi(config.GetSection("azureAd"));
//        return services;
//    }

//    private const string SectionName = "jwt";
//    private const string RegistryName = "auth";

//    public static IMameyBuilder AddAzureB2BJwt(this IMameyBuilder builder, string sectionName = SectionName,
//        Action<JwtBearerOptions> optionsFactory = null)
//    {
//        if (string.IsNullOrWhiteSpace(sectionName))
//        {
//            sectionName = SectionName;
//        }

//        var options = builder.GetOptions<JwtOptions>(sectionName);
//        var aadOptions = builder.GetOptions<AzureAdOptions>("azureAd");
//        return builder.AddAzureB2BJwt(options, aadOptions, optionsFactory);
//    }

//    private static IMameyBuilder AddAzureB2BJwt(this IMameyBuilder builder, JwtOptions options, AzureAdOptions azureAdOptions,
//        Action<JwtBearerOptions> optionsFactory = null)
//    {
//        if (!builder.TryRegister(RegistryName))
//        {
//            return builder;
//        }

//        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


//        if (options.AuthenticationDisabled)
//        {
//            builder.Services.AddSingleton<IPolicyEvaluator, DisabledAuthenticationPolicyEvaluator>();
//        }

//        var tokenValidationParameters = new TokenValidationParameters
//        {
//            RequireAudience = options.RequireAudience,
//            ValidIssuer = options.ValidIssuer,
//            ValidIssuers = options.ValidIssuers,
//            ValidateActor = options.ValidateActor,
//            ValidAudience = options.ValidAudience,
//            ValidAudiences = options.ValidAudiences,
//            ValidateAudience = options.ValidateAudience,
//            ValidateIssuer = options.ValidateIssuer,
//            ValidateLifetime = options.ValidateLifetime,
//            ValidateTokenReplay = options.ValidateTokenReplay,
//            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
//            SaveSigninToken = options.SaveSigninToken,
//            RequireExpirationTime = options.RequireExpirationTime,
//            RequireSignedTokens = options.RequireSignedTokens,
//            ClockSkew = TimeSpan.Zero
//        };

//        if (!string.IsNullOrWhiteSpace(options.AuthenticationType))
//        {
//            tokenValidationParameters.AuthenticationType = options.AuthenticationType;
//        }

//        var hasCertificate = false;
//        if (options.Certificate is not null)
//        {
//            X509Certificate2 certificate = null;
//            var password = options.Certificate.Password;
//            var hasPassword = !string.IsNullOrWhiteSpace(password);
//            if (!string.IsNullOrWhiteSpace(options.Certificate.Location))
//            {
//                certificate = hasPassword
//                    ? new X509Certificate2(options.Certificate.Location, password)
//                    : new X509Certificate2(options.Certificate.Location);
//                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
//                Console.WriteLine($"Loaded X.509 certificate from location: '{options.Certificate.Location}' {keyType}.");
//            }

//            if (!string.IsNullOrWhiteSpace(options.Certificate.RawData))
//            {
//                var rawData = Convert.FromBase64String(options.Certificate.RawData);
//                certificate = hasPassword
//                    ? new X509Certificate2(rawData, password)
//                    : new X509Certificate2(rawData);
//                var keyType = certificate.HasPrivateKey ? "with private key" : "with public key only";
//                Console.WriteLine($"Loaded X.509 certificate from raw data {keyType}.");
//            }

//            if (certificate is not null)
//            {
//                if (string.IsNullOrWhiteSpace(options.Algorithm))
//                {
//                    options.Algorithm = SecurityAlgorithms.RsaSha256;
//                }

//                hasCertificate = true;
//                tokenValidationParameters.IssuerSigningKey = new X509SecurityKey(certificate);
//                var actionType = certificate.HasPrivateKey ? "issuing" : "validating";
//                Console.WriteLine($"Using X.509 certificate for {actionType} tokens.");
//            }
//        }

//        if (!string.IsNullOrWhiteSpace(options.IssuerSigningKey) && !hasCertificate)
//        {
//            if (string.IsNullOrWhiteSpace(options.Algorithm) || hasCertificate)
//            {
//                options.Algorithm = SecurityAlgorithms.HmacSha256;
//            }

//            var rawKey = Encoding.UTF8.GetBytes(options.IssuerSigningKey);
//            tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);
//            Console.WriteLine("Using symmetric encryption for issuing tokens.");
//        }

//        if (!string.IsNullOrWhiteSpace(options.NameClaimType))
//        {
//            tokenValidationParameters.NameClaimType = options.NameClaimType;
//        }

//        if (!string.IsNullOrWhiteSpace(options.RoleClaimType))
//        {
//            tokenValidationParameters.RoleClaimType = options.RoleClaimType;
//        }
//        var config = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>();
//        var authenticationBuilder = builder.Services
//            .AddAuthentication(o =>
//            {
//                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//            });
//        authenticationBuilder
//            .AddMicrosoftIdentityWebApi(config.GetSection("azureAd"))
//                    //.EnableTokenAcquisitionToCallDownstreamApi()
//                    //    .AddMicrosoftGraph(config.GetSection("downstreamApi"))
//                    //    .AddInMemoryTokenCaches()
//                    ;
//        //authenticationBuilder.AddJwtBearer(o =>
//        //    {
//        //        o.Authority = $"{azureAdOptions.Instance}{azureAdOptions.TenantId}";
//        //        o.Audience = azureAdOptions.ClientId;
//        //        o.MetadataAddress = $"https://login.microsoftonline.com/{azureAdOptions.TenantId}/v2.0/.well-known/openid-configuration";
//        //        o.SaveToken = options.SaveToken;
//        //        o.RefreshOnIssuerKeyNotFound = options.RefreshOnIssuerKeyNotFound;
//        //        o.RequireHttpsMetadata = options.RequireHttpsMetadata;
//        //        o.IncludeErrorDetails = options.IncludeErrorDetails;
//        //        o.TokenValidationParameters = tokenValidationParameters;

//        //        if (!string.IsNullOrWhiteSpace(options.Challenge))
//        //        {
//        //            o.Challenge = options.Challenge;
//        //        }

//        //        optionsFactory?.Invoke(o);
//        //    });

//        builder.Services.AddSingleton(options);
//        builder.Services.AddSingleton(tokenValidationParameters);

//        return builder;
//    }
//}

using Mamey.Azure.Abstractions;
using Mamey.Graph;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace Mamey.Azure.Identity;
public static class Extensions
{
    public static IMameyBuilder AddAzureIdentity(this IMameyBuilder builder)
    {
        //builder.AddWebAssembyGraph();
        builder.Services.AddAzureIdentity();
        return builder;
    }
    public static IServiceCollection AddAzureIdentity(this IServiceCollection services,
        string section = AzureOptions.APPSETTINGS_SECTION)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        var azureAdOptions = services.GetOptions<AzureOptions>(section);

        services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "role";

            });
        services.AddAuthentication(sharedOptions =>
        {
            sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        //.AddJwtBearer(jwtOptions =>
        //{
        //    configuration.Bind("azureAd", jwtOptions);
        //    jwtOptions.Events = new JwtBearerEvents
        //    {
        //        OnAuthenticationFailed = AuthenticationFailed
        //    };
        //    jwtOptions.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidIssuer = configuration["azureAd:issuer"],
        //        ValidateAudience = true,
        //        ValidAudience = configuration["azureAd:audience"],
        //        ValidateLifetime = true,
        //        ClockSkew = TimeSpan.Zero
        //    };
        //})
        .AddCookie(options =>
        {
            options.LoginPath = "/authentication/login";
        }) // Adding Cookie Authentication
        //.AddOpenIdConnect(openIdOptions =>
        //{
            
        //    configuration.Bind("azureAd", openIdOptions);
        //    openIdOptions.Authority = $"https://login.microsoftonline.com/{azureAdOptions.TenantId}/v2.0";
        //    openIdOptions.ClientId = azureAdOptions.ClientId;
        //    openIdOptions.ClientSecret = azureAdOptions.ClientSecret;
        //    openIdOptions.CallbackPath = azureAdOptions.CallbackPath;



        //    openIdOptions.Events = new OpenIdConnectEvents
        //    {
        //        OnRemoteFailure = OnAuthenticationFailed
        //    };

       
        //    configuration.Bind("azureAd", openIdOptions);

        //    openIdOptions.RemoteAuthenticationTimeout = TimeSpan.FromSeconds(120);
        //    openIdOptions.ResponseType = OpenIdConnectResponseType.Code;
        //    openIdOptions.GetClaimsFromUserInfoEndpoint = true;
        //    openIdOptions.SaveTokens = true;
        //    openIdOptions.UseTokenLifetime = true;
        //    openIdOptions.SignInScheme = IdentityConstants.ApplicationScheme;
        //    openIdOptions.Scope.Add("openid");
        //    openIdOptions.Scope.Add("profile");
        //    openIdOptions.Scope.Add("email");
        //    openIdOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");

        //    openIdOptions.MetadataAddress = $"https://login.microsoftonline.com/{azureAdOptions.TenantId}/v2.0/.well-known/openid-configuration";
        //    openIdOptions.Validate();
        //})
        .AddMicrosoftIdentityWebApi(configuration.GetSection("azureAd"))
            .EnableTokenAcquisitionToCallDownstreamApi()
                .AddMicrosoftGraph(configuration.GetSection("downstreamApi"))
                .AddInMemoryTokenCaches();
        
        return services;
    }

    private static Task AuthenticationFailed(Microsoft.AspNetCore.Authentication.JwtBearer.AuthenticationFailedContext arg)
    {
        // Handle authentication failures here
        return Task.FromResult(0);
    }
    private static Task AuthenticationFailed(Microsoft.AspNetCore.Authentication.OpenIdConnect.AuthenticationFailedContext arg)
    {
        // Handle authentication failures here
        return Task.FromResult(0);
    }

    private static Task OnAuthenticationFailed(RemoteFailureContext context)
    {
        context.HandleResponse();
        context.Response.Redirect("/Error?message=" + context.Failure.Message);
        return Task.FromResult(0);
    }

    private static IMameyBuilder ConfigureServer(this IMameyBuilder builder, GraphOptions graphOptions)
    {
        switch (graphOptions.Type)
        {
            case "client":
                break;
            case "server":
                break;
            default:
                break;
        }
        
        return builder;
    }
    
    
    private static IMameyBuilder ConfigurePublicServer(this IMameyBuilder builder)
    {
        builder.Services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "role";

            });


        return builder;
    }
    private static IMameyBuilder ConfigureConfidentialServer(this IMameyBuilder builder)
    {
        builder.Services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "role";

            });


        return builder;
    }
}
