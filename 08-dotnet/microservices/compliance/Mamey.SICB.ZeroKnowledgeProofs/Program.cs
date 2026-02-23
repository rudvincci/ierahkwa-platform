using Mamey.SICB.ZeroKnowledgeProofs.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Mamey SICB Zero Knowledge Proofs API", Version = "v1" });
});

builder.Services.AddSingleton<IZKProofService, ZKProofService>();

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

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.SICB.ZeroKnowledgeProofs" }));

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║     MAMEY SICB ZERO KNOWLEDGE PROOFS SERVICE v1.0.0          ║");
Console.WriteLine("║     Privacy-Preserving Verification for Sovereign Network    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();
