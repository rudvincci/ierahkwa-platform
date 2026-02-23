using OutlookExtractor.Core.Interfaces;
using OutlookExtractor.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Ierahkwa Outlook Email Extractor - .NET 10
// Sovereign Government Communication Tool
// Office 365 / Outlook / Exchange Integration
// ========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Ierahkwa Outlook Email Extractor API",
        Version = "v1.0",
        Description = "Extract email addresses from Office 365, Outlook, Hotmail, and Exchange accounts. Export to Text and Excel files.",
        Contact = new()
        {
            Name = "Sovereign Government of Ierahkwa Ne Kanienke",
            Email = "tech@ierahkwa.gov"
        }
    });
});

// Register Services
builder.Services.AddSingleton<MicrosoftGraphService>();
builder.Services.AddScoped<IEmailExtractionService, EmailExtractionService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

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
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ierahkwa Outlook Extractor API v1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new
{
    status = "healthy",
    service = "Ierahkwa Outlook Email Extractor",
    version = "1.0.0",
    features = new[]
    {
        "Office 365 Integration",
        "Outlook / Hotmail / Live Support",
        "Exchange Server Support",
        "Email Address Extraction",
        "Calendar Event Extraction",
        "Contact List Extraction",
        "Text File Export",
        "Excel File Export",
        "Statistics & Analytics"
    },
    timestamp = DateTime.UtcNow
});

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║                                                               ║
║   📧  IERAHKWA OUTLOOK EMAIL EXTRACTOR                        ║
║   Sovereign Government Communication Tool                     ║
║                                                               ║
║   Microsoft 365 / Office 365 Integration                      ║
║   Outlook / Hotmail / Live / Exchange Support                ║
║                                                               ║
║   Features:                                                   ║
║   ✅ Extract emails from Inbox, Sent, Archive               ║
║   ✅ Extract from Calendar events                           ║
║   ✅ Extract from Contacts                                  ║
║   ✅ Export to Text File                                    ║
║   ✅ Export to Excel (XLSX) with formatting                 ║
║   ✅ Remove duplicates automatically                        ║
║   ✅ Statistics & Analytics                                 ║
║   ✅ Filter by date range                                   ║
║   ✅ Domain filtering                                       ║
║                                                               ║
║   API Documentation: http://localhost:5055/                  ║
║                                                               ║
║   Sovereign Government of Ierahkwa Ne Kanienke               ║
║   © 2026 All Rights Reserved                                  ║
║                                                               ║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
