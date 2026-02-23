using AdvocateOffice.Services;
using AdvocateOffice.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<DbService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() { Title = "Advocate Office API", Version = "v1" }); });
builder.Services.AddCors(o => o.AddDefaultPolicy(p => {
    var origins = Environment.GetEnvironmentVariable("CORS_ORIGINS")?.Split(',') ?? new[] { "http://localhost:3000" };
    p.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Advocate Office API v1"));
app.UseCors();
app.UseStaticFiles();
app.UseMiddleware<TokenAuthMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/index.html"));
app.MapGet("/health", () => new { status = "healthy", service = "Advocate Office Management System", version = "2.0", net = "10" });

Console.WriteLine(@"
╔═══════════════════════════════════════════════════════════════╗
║   ADVOCATE OFFICE MANAGEMENT SYSTEM — .NET 10                 ║
║   IGT-LEGAL | Sovereign Government Legal Platform             ║
╠═══════════════════════════════════════════════════════════════╣
║   Login: admin / [SET VIA DEFAULT_ADMIN_PASSWORD ENV]          ║
║   API:   /swagger   Dashboard: /index.html   Login: /login.html║
╚═══════════════════════════════════════════════════════════════╝
");

app.Run();
