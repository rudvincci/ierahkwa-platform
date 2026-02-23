using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Configuration;
using Mamey.Identity.Decentralized.Methods.Ethr;
using Mamey.Identity.Decentralized.Methods.Ion;
using Mamey.Identity.Decentralized.Methods.Web;
using Mamey.Identity.Decentralized.Middlewares;
using Mamey.Identity.Decentralized.Resolution;
using Mamey.Identity.Decentralized.Services;
using Mamey.Identity.Decentralized.Trust;
using Mamey.Identity.Decentralized.Validation;
using Mamey.Identity.Decentralized.VC;
using Mamey.Http;
using Nethereum.Web3;

namespace Mamey.Identity.Decentralized;

public static class Extensions
{
    private const string SectionName = "dids";
    private const string RegistryName = "auth.dids";

    public static IMameyBuilder AddDecentralizedIdentifiers(
        this IMameyBuilder builder,
        string sectionName = SectionName,
        string httpClientSectionName = "httpClient",
        Action<DecentralizedIdentifierOptions>? configure = null)
    {
        if (!builder.TryRegister(RegistryName)) return builder;

        sectionName = string.IsNullOrWhiteSpace(sectionName) ? SectionName : sectionName;

        var didOptions = builder.GetOptions<DecentralizedIdentifierOptions>(sectionName);
        builder.Services.AddSingleton(didOptions);
        var httpClientOptions = builder.GetOptions<HttpClientOptions>(httpClientSectionName);
        
        builder.Services.AddDecentralizedIdentifiers(builder.Configuration, configure);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DIDCorsPolicy", policy =>
                policy.AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()
                      .SetIsOriginAllowed(_ => true));
        });

        return builder;
    }

    public static IServiceCollection AddDecentralizedIdentifiers(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DecentralizedIdentifierOptions>? configure = null)
    {
        services.Configure<DecentralizedIdentifierOptions>(configuration.GetSection(DecentralizedIdentifierOptions.SectionName));
        if (configure is not null)
            services.Configure(configure);

        services.AddSingleton<IKeyProvider, KeyProvider>();
        services.AddSingleton<IBlockchainProvider, BlockchainProvider>();
        services.AddSingleton<ICredentialStatusService, CredentialStatusService>();

        return services;
    }

    public static IApplicationBuilder UseDecentralizedIdentifiers(this IApplicationBuilder app)
    {
        app.UseCors("DIDCorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<DidAuthMiddleware>();
        return app;
    }

    public static IMameyBuilder AddAllDidResolvers(this IMameyBuilder builder, string decentralizedIdentifierOptionsSectionName = null)
    {
        var options = builder.GetOptions<DecentralizedIdentifierOptions>(string.IsNullOrEmpty(decentralizedIdentifierOptionsSectionName) 
            ? DecentralizedIdentifierOptions.SectionName 
            : decentralizedIdentifierOptionsSectionName);
        
        builder.Services.AddHttpClient<DidWebResolver>();
        builder.Services.AddSingleton<IDidResolver, DidWebResolver>();

        
        if (options.Resolver.EnabledMethods.Contains("ion"))
        {
            builder.Services.AddHttpClient<DidIonResolver>();
            builder.Services.AddSingleton<IDidResolver, DidIonResolver>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("sample");
                options.Resolver.MethodEndpoints.TryGetValue("ion", out var methodEndpoint);
                if (string.IsNullOrEmpty(methodEndpoint?.Trim()))
                {
                    throw new Exception("there is no method endpoint for 'ion'");
                }
                return new DidIonResolver(client, methodEndpoint);
            });
        }
        
        if (options.Resolver.EnabledMethods.Contains("ethr"))
        {
            builder.Services.AddHttpClient<DidEthrResolver>();
            builder.Services.AddSingleton<IDidResolver, DidEthrResolver>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("sample");
                options.Resolver.MethodEndpoints.TryGetValue("ethr", out var methodEndpoint);
                if (string.IsNullOrEmpty(methodEndpoint?.Trim()))
                {
                    throw new Exception("there is no method endpoint for 'ethr'");
                }
                return new DidEthrResolver(client, methodEndpoint);
            });
        }
        // if (options.Resolver.EnabledMethods.Contains("key"))
        // {
        //     builder.Services.AddHttpClient<DidKeyResolver>();
        //     builder.Services.AddSingleton<IDidResolver, DidKeyResolver>(sp =>
        //     {
        //         var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("sample");
        //         options.Resolver.MethodEndpoints.TryGetValue("key", out var methodEndpoint);
        //         if (string.IsNullOrEmpty(methodEndpoint?.Trim()))
        //         {
        //             throw new Exception("there is no method endpoint for 'key'");
        //         }
        //         return new DidKeyResolver(client, methodEndpoint);
        //     });
        // }
        // if (options.Resolver.EnabledMethods.Contains("peer"))
        // {
        //     builder.Services.AddHttpClient<DidPeerResolver>();
        //     builder.Services.AddSingleton<IDidResolver, DidPeerResolver>(sp =>
        //     {
        //         var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("sample");
        //         options.Resolver.MethodEndpoints.TryGetValue("peer", out var methodEndpoint);
        //         if (string.IsNullOrEmpty(methodEndpoint?.Trim()))
        //         {
        //             throw new Exception("there is no method endpoint for 'peer'");
        //         }
        //         return new DidPeerResolver(client, methodEndpoint);
        //     });
        // }

        builder.Services.AddSingleton<IDidMethodRegistry, DidMethodRegistry>();
        builder.Services.AddSingleton<DidResolver>();

        builder.Services.AddSingleton<IDidResolver>(sp =>
        {
            var registry = sp.GetRequiredService<IDidMethodRegistry>();
            foreach (var method in sp.GetServices<IDidMethod>())
            {
                registry.Register(method);
            }

            return sp.GetRequiredService<IDidResolver>();
        });

        return builder;
    }

    public static IMameyBuilder AddJsonLd(this IMameyBuilder builder)
    {
        builder.Services.AddSingleton<IJsonLdProcessor, JsonLdProcessor>();
        builder.Services.AddSingleton<ICanonicalizationService, CanonicalizationService>();
        return builder;
    }

    public static IMameyBuilder AddProofValidation(this IMameyBuilder builder)
    {
        builder.Services.AddSingleton<IProofService, ProofService>();
        builder.Services.AddScoped<LinkedDataProofValidator>();
        builder.Services.AddSingleton<IVerifiableCredentialValidator, VerifiableCredentialValidator>();
        return builder;
    }

    public static IMameyBuilder AddTrustRegistry(
        this IMameyBuilder builder,
        Action<BlockchainTrustRegistryOptions>? configure = null)
    {
        builder.Services.Configure(configure ?? (_ => { }));

        builder.Services.AddSingleton<ITrustRegistry>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var options = sp.GetRequiredService<IOptions<BlockchainTrustRegistryOptions>>().Value;

            var registries = new List<ITrustRegistry>
            {
                new LocalTrustRegistry(),
                new HttpTrustRegistry("https://trust.sov.org/api/issuers", httpClient),
                new DnsTrustRegistry("_did.example.com"),
                new BlockchainTrustRegistry(new Web3(options.RpcUrl), options.ContractAddress)
            };

            return new MultiSourceTrustRegistry(registries);
        });

        return builder;
    }

    public static IMameyBuilder AddDidAuth(this IMameyBuilder builder)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeyResolver = (token, secToken, kid, parameters) =>
                    {
                        var keyProvider = builder.Services.BuildServiceProvider()
                                                         .GetRequiredService<IKeyProvider>();
                        var key = keyProvider.ResolveKey(kid);
                        return new[] { new SymmetricSecurityKey(key) { KeyId = kid } };
                    }
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {
                        var validator = ctx.HttpContext.RequestServices
                                                .GetRequiredService<LinkedDataProofValidator>();

                        if (ctx.SecurityToken is JwtSecurityToken jwtToken)
                        {
                            var valid = await validator.ValidateAsync(jwtToken.RawData);
                            if (!valid) ctx.Fail("VC/VP invalid.");
                        }
                        else
                        {
                            ctx.Fail("Unsupported token type.");
                        }
                    }
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireDID", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim(c => c.Type == "sub" && c.Value.StartsWith("did:"))));

            options.AddPolicy("RequireVC", policy =>
                policy.RequireClaim("vc"));
        });

        builder.Services.AddSingleton<IAuthorizationHandler, DidAuthorizationHandler>();
        builder.Services.AddTransient<DidAuthMiddleware>();

        return builder;
    }
}
