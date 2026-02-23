using FutureheadGroup.Landing.Blazor;
using FutureheadGroup.Landing.Blazor.Configuration;
using Mamey;
using Mamey.Casino.Infrastructure;
using Mamey.Casino.Infrastructure.Hubs;
using MudBlazor.Services;
using MudExtensions.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();
builder.Services.AddMudExtensions();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();


builder.Services
    .AddMamey(configuration:builder.Configuration, hostEnvironment:builder.Environment)
    .AddCasinoInfrastructure()
    .AddFutureheadGroup()
    .Build();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
var svcProv = scope.ServiceProvider;
await svcProv.GetRequiredService<FutureheadGroupGlobalSettings>().InitializeAsync();

app.UseHttpsRedirection();
app.MapControllers();
app.UseAntiforgery();
app.UseStaticFiles();
app.MapStaticAssets();

app.UseRouting();

await app.UseCasinoInfrastructureAsync();

app.MapControllers();
app.MapBlazorHub();
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapFallbackToPage("/_Host");
app.MapFutureheadGroupAsync();

await app.RunAsync();
