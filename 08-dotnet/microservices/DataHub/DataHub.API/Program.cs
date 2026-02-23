using DataHub.Core.Interfaces;
using DataHub.Infrastructure.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Ierahkwa DataHub API", Version = "v1" }));
builder.Services.AddSingleton<IDataHubService, DataHubService>();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseCors("AllowAll"); app.UseAuthorization(); app.MapControllers();
app.Run();
