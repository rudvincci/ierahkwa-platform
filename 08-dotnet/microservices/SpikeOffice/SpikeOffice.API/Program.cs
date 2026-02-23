using Microsoft.EntityFrameworkCore;
using SpikeOffice.Infrastructure.Data;
using SpikeOffice.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// Spike Office SaaS - .NET 10
// Multi-Tenant Payroll & Office Management
// Integrated with IERAHKWA Sovereign Government
// ========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Spike Office SaaS API", Version = "v1" });
});

builder.Services.AddSpikeOfficeInfrastructure(builder.Configuration);

builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p =>
    {
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Tenant resolution (path /t/{prefix}/..., header X-Tenant-Prefix, or ?tenant=)
app.UseSpikeOfficeTenantResolution();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spike Office SaaS v1"));
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Health & IERAHKWA integration info
app.MapGet("/health", () => new
{
    status = "healthy",
    service = "Spike Office SaaS",
    version = "1.0.0",
    ierahkwa = "Integrated",
    timestamp = DateTime.UtcNow
});

app.MapGet("/", () => Results.Redirect("/index.html"));

// Ensure DB + seed default tenant for IERAHKWA
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SpikeOfficeDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (!await db.Tenants.IgnoreQueryFilters().AnyAsync())
    {
        db.Tenants.Add(new SpikeOffice.Core.Entities.Tenant
        {
            Id = Guid.NewGuid(),
            Name = "IERAHKWA Government",
            UrlPrefix = "ierahkwa",
            DefaultLanguage = "en",
            IsActive = true,
            IerahkwaDepartmentCode = "PM",
            IgtTokenSymbol = "IGT-MAIN"
        });
        await db.SaveChangesAsync();
    }
}

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║  Spike Office SaaS – Multi-Tenant Payroll & Office            ║
║  .NET 10 | IERAHKWA Integrated                                ║
║  HR • Payroll • Accounting • Tasks • Attendance • Loans        ║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
