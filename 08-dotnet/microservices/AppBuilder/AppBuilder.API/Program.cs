using System.Text;
using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using AppBuilder.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ═══════════════════════════════════════════════════════════════════════════════
// IERAHKWA Appy – AI-Powered No-Code Mobile App Builder SaaS – .NET 10
// Sovereign Government of Ierahkwa Ne Kanienke
// ═══════════════════════════════════════════════════════════════════════════════

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IERAHKWA Appy API", Version = "v1.0", Description = "AI-Powered No-Code Mobile App Builder. Create apps from URL, AI assistant, subscriptions, push, GDPR." });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.Section));

var jwt = builder.Configuration.GetSection(JwtOptions.Section).Get<JwtOptions>() ?? new JwtOptions();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// Services
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<ISubscriptionService, SubscriptionService>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IInvoiceService, InvoiceService>();
builder.Services.AddSingleton<IPushNotificationService, PushNotificationService>();
builder.Services.AddSingleton<IAiAssistantService, AiAssistantService>();
builder.Services.AddSingleton<IPluginService, PluginService>();
builder.Services.AddSingleton<IAppBuilderService, AppBuilderService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IERAHKWA Appy API v1.0");
    c.DocumentTitle = "IERAHKWA Appy API";
});

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => new
{
    status = "healthy",
    service = "IERAHKWA Appy",
    version = "1.0",
    platform = "IERAHKWA Futurehead Platform",
    features = new[]
    {
        "AI-powered app config", "No-code from URL", "Browser preview", "PayPal & Bank",
        "Subscriptions (Free/Pro/Enterprise)", "Build credits", "Push notifications",
        "GDPR export/delete", "Android + WordPress plugin", "QR code distribution",
        "App signing", "Capacitor/PWA"
    },
    timestamp = DateTime.UtcNow
});

app.MapGet("/", () => Results.Redirect("/index.html"));

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║   IERAHKWA APPY – AI-Powered No-Code App Builder              ║
║   .NET 10 · SaaS · Sovereign Government of Ierahkwa           ║
║                                                               ║
║   • Enter URL → Create app in 5 min · AI assistant            ║
║   • PayPal & Bank Transfer · Free / Pro / Enterprise          ║
║   • Browser preview · QR · Push · GDPR compliant              ║
║   • Android (included) · WordPress (plugin)                   ║
║                                                               ║
║   Sovereign Government of Ierahkwa Ne Kanienke                ║
║   © 2026 All Rights Reserved                                  ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
