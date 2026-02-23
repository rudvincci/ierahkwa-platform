using RnBCal.Core.Interfaces;
using RnBCal.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// IERAHKWA RnBCal - .NET 10
// Rental & Booking Calendar Sync System
// ========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IERAHKWA RnBCal API", Version = "v1.0.4" });
});

// Configure Email
var emailConfig = new EmailConfiguration
{
    Enabled = builder.Configuration.GetValue<bool>("Email:Enabled"),
    SmtpHost = builder.Configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
    SmtpPort = builder.Configuration.GetValue<int>("Email:SmtpPort", 587),
    UseSsl = builder.Configuration.GetValue<bool>("Email:UseSsl", true),
    Username = builder.Configuration["Email:Username"] ?? "",
    Password = builder.Configuration["Email:Password"] ?? "",
    FromEmail = builder.Configuration["Email:FromEmail"] ?? "bookings@ierahkwa.gov",
    FromName = builder.Configuration["Email:FromName"] ?? "IERAHKWA Booking System"
};

// Configure Google Calendar
var googleConfig = new GoogleCalendarConfig
{
    Enabled = builder.Configuration.GetValue<bool>("GoogleCalendar:Enabled"),
    ClientId = builder.Configuration["GoogleCalendar:ClientId"] ?? "",
    ClientSecret = builder.Configuration["GoogleCalendar:ClientSecret"] ?? "",
    RedirectUri = builder.Configuration["GoogleCalendar:RedirectUri"] ?? "http://localhost:5055/api/calendar/google/callback",
    AccessToken = builder.Configuration["GoogleCalendar:AccessToken"] ?? "",
    RefreshToken = builder.Configuration["GoogleCalendar:RefreshToken"] ?? "",
    CalendarId = builder.Configuration["GoogleCalendar:CalendarId"] ?? "primary"
};

builder.Services.AddSingleton(emailConfig);
builder.Services.AddSingleton(googleConfig);

// Register Services
builder.Services.AddSingleton<ICalendarService, CalendarService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHttpClient<IGoogleCalendarService, GoogleCalendarService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure Pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IERAHKWA RnBCal API v1.0.4");
    c.DocumentTitle = "IERAHKWA RnBCal API";
});

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Health check
app.MapGet("/health", () => new
{
    status = "healthy",
    service = "IERAHKWA RnBCal",
    version = "1.0.4",
    platform = "IERAHKWA Futurehead Platform",
    features = new[]
    {
        "ICS File Generation",
        "Multi-Provider Calendar Links (Google, Yahoo, Outlook, Office365, Apple, AOL)",
        "Email Integration with Calendar Attachments",
        "Google Calendar Auto-Sync",
        "Booking Management"
    },
    timestamp = DateTime.UtcNow
});

// Root endpoint
app.MapGet("/", () => Results.Redirect("/index.html"));

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║   📆  IERAHKWA RnBCAL                                         ║
║   Rental & Booking Calendar Sync System                       ║
║                                                               ║
║   Version: 1.0.4 (October 2025 Release)                       ║
║                                                               ║
║   Features:                                                   ║
║   ✅ ICS File Generation (RFC 5545 Compliant)               ║
║   ✅ Google Calendar Integration & Auto-Sync                ║
║   ✅ Yahoo Calendar Direct Links                            ║
║   ✅ Outlook & Office365 Calendar Links                     ║
║   ✅ Apple Calendar (.ics Download)                         ║
║   ✅ AOL Calendar Support                                   ║
║   ✅ Email Notifications with Attachments                   ║
║   ✅ Booking Management API                                 ║
║                                                               ║
║   Supported Rentals:                                          ║
║   • Car Rentals      • Bike Rentals                          ║
║   • Yacht Rentals    • Hotel Rooms                           ║
║   • Airbnb Properties • Equipment                            ║
║                                                               ║
║   🏛️  Sovereign Government of Ierahkwa Ne Kanienke          ║
║   © 2026 All Rights Reserved                                  ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
