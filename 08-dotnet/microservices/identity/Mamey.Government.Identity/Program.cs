using Mamey.Government.Identity.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Mamey Government Identity API", Version = "v1" });
});

builder.Services.AddSingleton<IIdentityService, IdentityService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.Government.Identity" }));

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║        MAMEY GOVERNMENT IDENTITY SERVICE v1.0.0              ║");
Console.WriteLine("║        FutureWampumID Authentication & KYC                   ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();
