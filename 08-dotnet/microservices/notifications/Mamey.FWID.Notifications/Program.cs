using Mamey.FWID.Notifications.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Mamey FWID Notifications API", Version = "v1" });
});

builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.FWID.Notifications" }));

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║        MAMEY FWID NOTIFICATIONS SERVICE v1.0.0               ║");
Console.WriteLine("║        Push | Email | SMS | In-App | Webhooks                ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();
