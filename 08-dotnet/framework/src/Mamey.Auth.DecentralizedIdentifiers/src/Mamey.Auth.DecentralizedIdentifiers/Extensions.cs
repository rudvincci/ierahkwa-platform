using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Configuration;
using Mamey.Auth.DecentralizedIdentifiers.Methods.Ethr;
using Mamey.Auth.DecentralizedIdentifiers.Methods.Ion;
using Mamey.Auth.DecentralizedIdentifiers.Methods.Peer;
using Mamey.Auth.DecentralizedIdentifiers.Methods.Web;
using Mamey.Auth.DecentralizedIdentifiers.Middlewares;
using Mamey.Auth.DecentralizedIdentifiers.Resolution;
using Mamey.Auth.DecentralizedIdentifiers.Services;
using Mamey.Auth.DecentralizedIdentifiers.Trust;
using Mamey.Auth.DecentralizedIdentifiers.Validation;
using Mamey.Auth.DecentralizedIdentifiers.VC;
using Mamey.Auth.DecentralizedIdentifiers.Handlers;
using Mamey.Auth.DecentralizedIdentifiers.Caching;
using Mamey.Auth.DecentralizedIdentifiers.Services;
using Mamey.Auth.DecentralizedIdentifiers.Audit;
using Mamey.Persistence.Redis;
using Mamey.Http;
using Microsoft.Extensions.Logging;
using Nethereum.Web3;

namespace Mamey.Auth.DecentralizedIdentifiers;

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

        // Register all required dependencies automatically
        // Register DID document cache - use Redis if available, otherwise fall back to memory cache
        builder.Services.AddMemoryCache(); // Always add memory cache as fallback
        
        // Check if Redis ICache is already registered (via AddRedis)
        var hasRedisCache = builder.Services.Any(s => s.ServiceType == typeof(ICache));
        
        if (hasRedisCache)
        {
            // Redis is available - use RedisDidDocumentCache
            builder.Services.AddSingleton<IDidDocumentCache>(sp => 
                new RedisDidDocumentCache(sp.GetRequiredService<ICache>(), 
                    sp.GetRequiredService<ILogger<RedisDidDocumentCache>>()));
        }
        else
        {
            // Redis not available - use MemoryDidDocumentCache
            builder.Services.AddSingleton<IDidDocumentCache, MemoryDidDocumentCache>();
        }
        
        // Register JSON-LD processor (required by VC services)
        builder.Services.AddSingleton<IJsonLdProcessor, JsonLdProcessor>();
        builder.Services.AddSingleton<ICanonicalizationService, CanonicalizationService>();
        
        // Register proof validation services (required by VC services)
        builder.Services.AddSingleton<IProofService, ProofService>();
        builder.Services.AddScoped<LinkedDataProofValidator>();
        builder.Services.AddSingleton<IVerifiableCredentialValidator, VerifiableCredentialValidator>();
        
        // Register audit services (required by VC services)
        builder.Services.AddSingleton<IAuditRepository, InMemoryAuditRepository>();
        builder.Services.AddSingleton<IDidAuditService, DidAuditService>();
        
        // Register DID resolvers
        builder.AddAllDidResolvers(sectionName);
        
        // Register DID services without authentication (JWT is already registered via AddMicroserviceSharedInfrastructure)
        // Only register the DID-specific services, not the authentication scheme
        var didAuthOptions = builder.GetOptions<DidAuthOptions>(sectionName);
        builder.Services.AddSingleton(didAuthOptions);
        
        // Register DID handler
        builder.Services.AddSingleton<IDidHandler, DidHandler>();
        
        // Register access token services
        builder.Services.AddSingleton<Services.IAccessTokenService, InMemoryDidAccessTokenService>();
        
        // Register caching services (use existing cache if available)
        builder.Services.AddSingleton<ICacheFactory, DefaultCacheFactory>();
        builder.Services.AddSingleton<IDidDocumentCache>(sp =>
        {
            var factory = sp.GetRequiredService<ICacheFactory>();
            var options = sp.GetRequiredService<DidAuthOptions>();
            return factory.CreateDidDocumentCache(new CacheOptions
            {
                Enabled = options.CacheOptions.Enabled,
                StorageType = options.CacheOptions.StorageType,
                TtlMinutes = options.CacheOptions.TtlMinutes,
                RedisConnectionString = options.CacheOptions.RedisConnectionString
            });
        });
        
        // Register audit services (if not already registered)
        if (!builder.Services.Any(s => s.ServiceType == typeof(IAuditRepository)))
        {
            builder.Services.AddSingleton<IAuditRepository, InMemoryAuditRepository>();
        }
        if (!builder.Services.Any(s => s.ServiceType == typeof(IDidAuditService)))
        {
            builder.Services.AddSingleton<IDidAuditService, DidAuditService>();
        }
        
        // Register DID authentication middleware (without authentication scheme registration)
        builder.Services.AddTransient<DidAuthMiddleware>();

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
        
        // Register VC services
        services.AddSingleton<ICredentialService, VCIssuanceService>();
        services.AddSingleton<IProofServiceFactory, ProofServiceFactory>();
        services.AddSingleton<IVPService, VPService>();

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
        
        // Register Bitcoin anchoring service
        builder.Services.AddHttpClient<IBitcoinAnchoringService, BitcoinAnchoringService>();

        builder.Services.AddHttpClient<DidWebResolver>();
        builder.Services.AddSingleton<IDidResolver, DidWebResolver>();

        
        if (options.Resolver.EnabledMethods.Contains("ion"))
        {
            builder.Services.AddHttpClient<DidIonResolver>();
            builder.Services.AddSingleton<IDidResolver, DidIonResolver>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("sample");
                var bitcoinService = sp.GetRequiredService<IBitcoinAnchoringService>();
                var logger = sp.GetRequiredService<ILogger<DidIonResolver>>();
                options.Resolver.MethodEndpoints.TryGetValue("ion", out var methodEndpoint);
                if (string.IsNullOrEmpty(methodEndpoint?.Trim()))
                {
                    throw new Exception("there is no method endpoint for 'ion'");
                }
                return new DidIonResolver(client, methodEndpoint, bitcoinService, logger);
            });
        }
        
        if (options.Resolver.EnabledMethods.Contains("ethr"))
        {
            builder.Services.AddHttpClient<DidEthrResolver>();
            builder.Services.AddSingleton<IDidResolver, DidEthrResolver>(sp =>
            {
                var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("sample");
                var documentCache = sp.GetRequiredService<IDidDocumentCache>();
                var logger = sp.GetRequiredService<ILogger<DidEthrResolver>>();
                options.Resolver.MethodEndpoints.TryGetValue("ethr", out var methodEndpoint);
                if (string.IsNullOrEmpty(methodEndpoint?.Trim()))
                {
                    throw new Exception("there is no method endpoint for 'ethr'");
                }
                return new DidEthrResolver(client, methodEndpoint, documentCache, logger);
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
        if (options.Resolver.EnabledMethods.Contains("peer"))
        {
            builder.Services.AddSingleton<IDidResolver, DidPeerResolver>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DidPeerResolver>>();
                return new DidPeerResolver(logger);
            });
        }

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

    /// <summary>
    /// Adds DID authentication with JWT-like capabilities similar to AddJwt
    /// </summary>
    public static IMameyBuilder AddDid(
        this IMameyBuilder builder,
        string sectionName = "didAuth",
        Action<DidAuthOptions>? optionsFactory = null)
    {
        if (!builder.TryRegister("auth.did")) return builder;

        var didAuthOptions = builder.GetOptions<DidAuthOptions>(sectionName);
        builder.Services.AddSingleton(didAuthOptions);

        // Register DID handler
        builder.Services.AddSingleton<IDidHandler, DidHandler>();

        // Register access token services
        builder.Services.AddSingleton<Services.IAccessTokenService, InMemoryDidAccessTokenService>();
        
        // Register caching services
        builder.Services.AddSingleton<ICacheFactory, DefaultCacheFactory>();
        builder.Services.AddSingleton<IDidDocumentCache>(sp =>
        {
            var factory = sp.GetRequiredService<ICacheFactory>();
            var options = sp.GetRequiredService<DidAuthOptions>();
            return factory.CreateDidDocumentCache(new CacheOptions
            {
                Enabled = options.CacheOptions.Enabled,
                StorageType = options.CacheOptions.StorageType,
                TtlMinutes = options.CacheOptions.TtlMinutes,
                RedisConnectionString = options.CacheOptions.RedisConnectionString
            });
        });
        
        builder.Services.AddSingleton<IVerificationResultCache>(sp =>
        {
            var factory = sp.GetRequiredService<ICacheFactory>();
            var options = sp.GetRequiredService<DidAuthOptions>();
            return factory.CreateVerificationResultCache(new CacheOptions
            {
                Enabled = options.CacheOptions.Enabled,
                StorageType = options.CacheOptions.StorageType,
                TtlMinutes = 30, // Shorter TTL for verification results
                RedisConnectionString = options.CacheOptions.RedisConnectionString
            });
        });
        
        builder.Services.AddSingleton<ICredentialStatusCache>(sp =>
        {
            var factory = sp.GetRequiredService<ICacheFactory>();
            var options = sp.GetRequiredService<DidAuthOptions>();
            return factory.CreateCredentialStatusCache(new CacheOptions
            {
                Enabled = options.CacheOptions.Enabled,
                StorageType = options.CacheOptions.StorageType,
                TtlMinutes = 15, // Shorter TTL for credential status
                RedisConnectionString = options.CacheOptions.RedisConnectionString
            });
        });

        // Register audit services
        builder.Services.AddSingleton<IAuditRepository, InMemoryAuditRepository>();
        builder.Services.AddSingleton<IDidAuditService, DidAuditService>();

        // Configure authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = didAuthOptions.Issuer,
                ValidAudience = didAuthOptions.Audience,
                IssuerSigningKeyResolver = (token, secToken, kid, parameters) =>
                {
                    var didHandler = builder.Services.BuildServiceProvider()
                                                   .GetRequiredService<IDidHandler>();
                    return didHandler.ResolveSigningKey(kid);
                }
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async ctx =>
                {
                    var didHandler = ctx.HttpContext.RequestServices
                                                  .GetRequiredService<IDidHandler>();
                    
                    if (ctx.SecurityToken is JwtSecurityToken jwtToken)
                    {
                        var isValid = await didHandler.ValidateDidToken(jwtToken.RawData);
                        if (!isValid) ctx.Fail("Invalid DID token.");
                    }
                    else
                    {
                        ctx.Fail("Unsupported token type.");
                    }
                }
            };

            optionsFactory?.Invoke(didAuthOptions);
        });

        // Configure authorization policies
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireDID", policy =>
                policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim(c => c.Type == "sub" && c.Value.StartsWith("did:"))));

            options.AddPolicy("RequireCitizenshipCredential", policy =>
                policy.RequireClaim("credentialType", "CitizenshipCredential"));

            options.AddPolicy("RequireAdminPermission", policy =>
                policy.RequireClaim("permissions", "admin"));
        });

        return builder;
    }

    /// <summary>
    /// Uses DID authentication middleware similar to UseJwt
    /// </summary>
    public static IApplicationBuilder UseDid(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.Use(async (ctx, next) =>
        {
            // Handle cookie-based DID authentication
            if (!ctx.Request.Headers.ContainsKey("Authorization"))
            {
                var cookieName = "__did-access-token";
                if (ctx.Request.Cookies.TryGetValue(cookieName, out var token))
                {
                    ctx.Request.Headers.Add("Authorization", $"Bearer {token}");
                }
            }
            await next();
        });
        app.UseAuthorization();
        app.UseMiddleware<DidAuthMiddleware>();
        return app;
    }
}
