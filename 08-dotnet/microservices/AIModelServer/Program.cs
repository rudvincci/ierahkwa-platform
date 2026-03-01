using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- Authentication ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.Authority = builder.Configuration["Jwt:Authority"] ?? "https://auth.ierahkwa.org";
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "https://auth.ierahkwa.org",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ierahkwa-api",
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

// --- CORS ---
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// --- Prometheus Metrics ---
var requestCounter = 0L;
var downloadCounter = 0L;

// --- Model Registry (mirrors ierahkwa-ai.js MODEL_REGISTRY) ---
var models = new[]
{
    new ModelManifest("sentiment-es",    "Sentiment Analysis (ES/EN)",        "1.0.0", "sentiment-multilingual-minilm.onnx", 25_600_000, "a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2", "text",      "sentiment",      "sentiment-tokenizer.json"),
    new ModelManifest("classify-multi",  "Text Classification (Multilingual)","1.0.0", "classify-distilbert-multi.onnx",     30_720_000, "b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3", "text",      "classification", "classify-tokenizer.json"),
    new ModelManifest("translate-es-en", "Translation ES -> EN",              "1.0.0", "translate-marian-es-en.onnx",        40_960_000, "c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4", "translate", "translation",    "translate-es-en-tokenizer.json"),
    new ModelManifest("translate-en-es", "Translation EN -> ES",              "1.0.0", "translate-marian-en-es.onnx",        40_960_000, "d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5", "translate", "translation",    "translate-en-es-tokenizer.json"),
    new ModelManifest("image-classify",  "Image Classification",              "1.0.0", "mobilenetv3-small.onnx",             10_240_000, "e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6", "image",     "classification", null),
    new ModelManifest("ner-multi",       "Named Entity Recognition",          "1.0.0", "ner-mbert-multi.onnx",               46_080_000, "f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1", "text",      "ner",            "ner-tokenizer.json"),
    new ModelManifest("embed-mini",      "Semantic Embeddings",               "1.0.0", "embed-minilm-l6-v2.onnx",            25_600_000, "a1c3e5b2d4f6a1c3e5b2d4f6a1c3e5b2d4f6a1c3e5b2d4f6a1c3e5b2d4f6a1c3", "text",      "embedding",      "embed-tokenizer.json")
};

var modelIndex = models.ToDictionary(m => m.Id);
var modelsBasePath = builder.Configuration["ModelsPath"] ?? "/data/models";

// --- GET /health (no auth) ---
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "ai-model-server",
    version = "1.0.0",
    timestamp = DateTime.UtcNow,
    uptime = Environment.TickCount64 / 1000.0
}));

// --- GET /stats ---
app.MapGet("/stats", () =>
{
    Interlocked.Increment(ref requestCounter);
    return Results.Ok(new
    {
        modelsAvailable = models.Length,
        totalSizeBytes = models.Sum(m => m.Size),
        downloads = Interlocked.Read(ref downloadCounter),
        requestsServed = Interlocked.Read(ref requestCounter)
    });
}).RequireAuthorization();

// --- GET /ai/models ---
app.MapGet("/ai/models", () =>
{
    Interlocked.Increment(ref requestCounter);
    return Results.Ok(models);
}).RequireAuthorization();

// --- GET /ai/models/{id}/manifest ---
app.MapGet("/ai/models/{id}/manifest", (string id) =>
{
    Interlocked.Increment(ref requestCounter);
    return modelIndex.TryGetValue(id, out var manifest)
        ? Results.Ok(manifest)
        : Results.NotFound(new { error = new { code = "MODEL_NOT_FOUND", message = $"Model '{id}' not found" } });
}).RequireAuthorization();

// --- GET /ai/models/{id}/download ---
app.MapGet("/ai/models/{id}/download", (string id) =>
{
    Interlocked.Increment(ref requestCounter);
    if (!modelIndex.TryGetValue(id, out var manifest))
        return Results.NotFound(new { error = new { code = "MODEL_NOT_FOUND", message = $"Model '{id}' not found" } });

    var filePath = Path.Combine(modelsBasePath, manifest.File);
    if (!File.Exists(filePath))
        return Results.NotFound(new { error = new { code = "FILE_NOT_FOUND", message = $"Model file for '{id}' not available on disk" } });

    Interlocked.Increment(ref downloadCounter);
    return Results.File(filePath, "application/octet-stream", manifest.File);
}).RequireAuthorization();

// --- GET /ai/models/{id}/tokenizer ---
app.MapGet("/ai/models/{id}/tokenizer", (string id) =>
{
    Interlocked.Increment(ref requestCounter);
    if (!modelIndex.TryGetValue(id, out var manifest))
        return Results.NotFound(new { error = new { code = "MODEL_NOT_FOUND", message = $"Model '{id}' not found" } });

    if (manifest.Tokenizer is null)
        return Results.NotFound(new { error = new { code = "NO_TOKENIZER", message = $"Model '{id}' does not have a tokenizer" } });

    var filePath = Path.Combine(modelsBasePath, manifest.Tokenizer);
    if (!File.Exists(filePath))
        return Results.NotFound(new { error = new { code = "FILE_NOT_FOUND", message = $"Tokenizer file for '{id}' not available on disk" } });

    return Results.File(filePath, "application/json", manifest.Tokenizer);
}).RequireAuthorization();

// --- GET /metrics (Prometheus) ---
app.MapGet("/metrics", () =>
{
    var totalSize = models.Sum(m => m.Size);
    var output = $"""
        # HELP aimodelserver_requests_total Total requests served
        # TYPE aimodelserver_requests_total counter
        aimodelserver_requests_total {Interlocked.Read(ref requestCounter)}
        # HELP aimodelserver_downloads_total Total model downloads
        # TYPE aimodelserver_downloads_total counter
        aimodelserver_downloads_total {Interlocked.Read(ref downloadCounter)}
        # HELP aimodelserver_models_available Number of models available
        # TYPE aimodelserver_models_available gauge
        aimodelserver_models_available {models.Length}
        # HELP aimodelserver_models_total_bytes Total size of all models in bytes
        # TYPE aimodelserver_models_total_bytes gauge
        aimodelserver_models_total_bytes {totalSize}
        """;
    return Results.Text(output, "text/plain; version=0.0.4; charset=utf-8");
});

app.Run();

// --- Record ---
record ModelManifest(
    string Id,
    string Name,
    string Version,
    string File,
    long Size,
    string Sha256,
    string Type,
    string Task,
    string? Tokenizer
);
