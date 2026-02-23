using Mamey.SICB.Treasury.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Mamey SICB Treasury API", Version = "v1" });
});

builder.Services.AddSingleton<ITreasuryService, TreasuryService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.SICB.Treasury" }));

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║         MAMEY SICB TREASURY SERVICE v1.0.0                   ║");
Console.WriteLine("║     Sovereign Indigenous Central Bank Operations             ║");
Console.WriteLine("║     Disbursements | Issuances | Reserves | Compliance        ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();
