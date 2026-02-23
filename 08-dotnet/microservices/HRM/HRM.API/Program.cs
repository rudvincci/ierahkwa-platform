using HRM.Core.Interfaces;
using HRM.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// IERAHKWA HRM - .NET 10
// Human Resource Management System
// Sovereign Government of Ierahkwa Ne Kanienke
// ========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "IERAHKWA HRM API", Version = "v1" });
});

builder.Services.AddSingleton<IHRMService, HRMService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IERAHKWA HRM API v1"));
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => new
{
    status = "healthy",
    service = "IERAHKWA HRM",
    version = "1.0.0",
    platform = "IERAHKWA",
    timestamp = DateTime.UtcNow
});

app.MapGet("/", () => Results.Redirect("/index.html"));

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║   IERAHKWA HRM - Human Resource Management System            ║
║   .NET 10 | Sovereign Government Digital Ecosystem           ║
║   Dashboard • Attendance • Leave • Payroll • Recruitment      ║
║   Loans • Awards • Projects • Procurement • Reports           ║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
