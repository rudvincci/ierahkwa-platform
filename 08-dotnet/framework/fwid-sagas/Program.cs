using Mamey.FWID.Sagas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Mamey FWID Sagas API", Version = "v1" }));
builder.Services.AddSingleton<ISagaService, SagaService>();
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.FWID.Sagas" }));

// Saga endpoints
app.MapPost("/api/v1/sagas", async (ISagaService svc, CreateSagaRequest req) =>
{
    var saga = await svc.CreateAsync(req.SagaType, req.InitialState, req.InitiatorId);
    return Results.Ok(new { success = true, saga });
});

app.MapGet("/api/v1/sagas/{id:guid}", async (ISagaService svc, Guid id) =>
{
    var saga = await svc.GetAsync(id);
    return saga != null ? Results.Ok(new { saga }) : Results.NotFound();
});

app.MapPost("/api/v1/sagas/{id:guid}/start", async (ISagaService svc, Guid id) =>
{
    var result = await svc.StartAsync(id);
    return Results.Ok(new { success = result });
});

app.MapPost("/api/v1/sagas/{id:guid}/steps/{stepIndex:int}/complete", async (ISagaService svc, Guid id, int stepIndex, CompleteStepRequest? req) =>
{
    var result = await svc.CompleteStepAsync(id, stepIndex, req?.Output);
    return Results.Ok(new { success = result });
});

app.MapPost("/api/v1/sagas/{id:guid}/steps/{stepIndex:int}/fail", async (ISagaService svc, Guid id, int stepIndex, FailStepRequest req) =>
{
    var result = await svc.FailStepAsync(id, stepIndex, req.ErrorMessage);
    return Results.Ok(new { success = result });
});

app.MapPost("/api/v1/sagas/{id:guid}/compensate", async (ISagaService svc, Guid id) =>
{
    var result = await svc.CompensateAsync(id);
    return Results.Ok(new { success = result });
});

app.MapPost("/api/v1/sagas/{id:guid}/cancel", async (ISagaService svc, Guid id) =>
{
    var result = await svc.CancelAsync(id);
    return Results.Ok(new { success = result });
});

app.MapGet("/api/v1/sagas/definitions/{sagaType}", async (ISagaService svc, string sagaType) =>
{
    var def = await svc.GetDefinitionAsync(sagaType);
    return def != null ? Results.Ok(new { definition = def }) : Results.NotFound();
});

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║          MAMEY FWID SAGAS SERVICE v1.0.0                     ║");
Console.WriteLine("║          Workflow Orchestration with Compensation            ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();

record CreateSagaRequest(string SagaType, Dictionary<string, object> InitialState, string? InitiatorId = null);
record CompleteStepRequest(string? Output);
record FailStepRequest(string ErrorMessage);
