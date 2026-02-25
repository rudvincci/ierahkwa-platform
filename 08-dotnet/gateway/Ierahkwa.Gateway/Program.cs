using Ierahkwa.Gateway.Auth;
using Ierahkwa.Gateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "Ierahkwa-Sovereign-Platform-Secret-Key-2026-Min32Chars!!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ierahkwa.sovereign";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("MemberOnly", p => p.RequireClaim("tier", "member", "resident", "citizen"));
    opt.AddPolicy("ResidentOnly", p => p.RequireClaim("tier", "resident", "citizen"));
    opt.AddPolicy("CitizenOnly", p => p.RequireClaim("tier", "citizen"));
    opt.AddPolicy("AdminOnly", p => p.RequireClaim("role", "admin"));
});

// YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Auth endpoints
builder.Services.AddSingleton(new JwtTokenService(jwtKey, jwtIssuer));

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

// Tenant resolution middleware (before auth)
app.UseMiddleware<TenantResolutionMiddleware>();

// Rate limiting middleware
app.UseMiddleware<RateLimitingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Health check
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    platform = "Ierahkwa Sovereign Platform",
    version = "3.0.0",
    microservices = 83,
    timestamp = DateTime.UtcNow
}));

// Auth endpoints
app.MapPost("/auth/login", (LoginRequest req, JwtTokenService jwt) =>
{
    // In production, validate against FWID identity service
    var token = jwt.GenerateToken(req.UserId, req.TenantId, req.Tier, req.Roles);
    return Results.Ok(new { token, expiresIn = 86400, tier = req.Tier });
});

app.MapPost("/auth/refresh", (RefreshRequest req, JwtTokenService jwt) =>
{
    var principal = jwt.ValidateToken(req.Token);
    if (principal == null) return Results.Unauthorized();
    var newToken = jwt.GenerateTokenFromPrincipal(principal);
    return Results.Ok(new { token = newToken, expiresIn = 86400 });
});

app.MapGet("/auth/me", (HttpContext ctx) =>
{
    if (ctx.User.Identity?.IsAuthenticated != true) return Results.Unauthorized();
    return Results.Ok(new
    {
        userId = ctx.User.FindFirst("sub")?.Value,
        tenantId = ctx.User.FindFirst("tenant")?.Value,
        tier = ctx.User.FindFirst("tier")?.Value,
        roles = ctx.User.FindAll("role").Select(c => c.Value)
    });
}).RequireAuthorization();

// Service discovery endpoint
app.MapGet("/services", () => Results.Ok(new
{
    gateway = "https://api.ierahkwa.org",
    services = GatewayRoutes.GetServiceCatalog()
}));

// YARP reverse proxy
app.MapReverseProxy();

app.Run();

public record LoginRequest(string UserId, string TenantId, string Tier, string[] Roles);
public record RefreshRequest(string Token);
