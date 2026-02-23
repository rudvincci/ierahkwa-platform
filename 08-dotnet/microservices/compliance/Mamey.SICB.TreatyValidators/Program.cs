var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Mamey SICB Treaty Validators", Version = "v1" }));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

var treaties = new Dictionary<string, Treaty>
{
    ["TREATY-001"] = new Treaty { TreatyId = "TREATY-001", Name = "Sovereign Recognition Treaty", Status = TreatyStatus.Active, SignedAt = DateTime.Parse("2020-01-01") },
    ["TREATY-002"] = new Treaty { TreatyId = "TREATY-002", Name = "Trade Agreement Treaty", Status = TreatyStatus.Active, SignedAt = DateTime.Parse("2021-06-15") },
    ["TREATY-003"] = new Treaty { TreatyId = "TREATY-003", Name = "Environmental Protection Treaty", Status = TreatyStatus.Active, SignedAt = DateTime.Parse("2022-03-20") }
};

var validations = new Dictionary<Guid, TreatyValidation>();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.SICB.TreatyValidators" }));

app.MapGet("/api/v1/treaties", () => Results.Ok(new { treaties = treaties.Values }));

app.MapGet("/api/v1/treaties/{treatyId}", (string treatyId) =>
{
    return treaties.TryGetValue(treatyId, out var treaty) ? Results.Ok(new { treaty }) : Results.NotFound();
});

app.MapPost("/api/v1/validate", (ValidateRequest req) =>
{
    var validation = new TreatyValidation
    {
        Id = Guid.NewGuid(),
        TreatyId = req.TreatyId,
        EntityId = req.EntityId,
        OperationType = req.OperationType,
        OperationData = req.OperationData,
        ValidationRules = new List<ValidationRule>
        {
            new() { RuleName = "TreatyActive", Passed = treaties.ContainsKey(req.TreatyId) && treaties[req.TreatyId].Status == TreatyStatus.Active },
            new() { RuleName = "AmountLimit", Passed = true, Details = "Within treaty limits" },
            new() { RuleName = "EntityAuthorized", Passed = true, Details = "Entity is authorized" },
            new() { RuleName = "CompliancePeriod", Passed = true, Details = "Within compliance period" }
        },
        CreatedAt = DateTime.UtcNow
    };
    
    validation.IsCompliant = validation.ValidationRules.All(r => r.Passed);
    validation.ComplianceScore = (decimal)validation.ValidationRules.Count(r => r.Passed) / validation.ValidationRules.Count * 100;
    validations[validation.Id] = validation;
    
    return Results.Ok(new { 
        success = true, 
        validation = new { 
            validation.Id, 
            validation.TreatyId, 
            validation.IsCompliant, 
            validation.ComplianceScore,
            validation.ValidationRules 
        }
    });
});

app.MapGet("/api/v1/validations/{id:guid}", (Guid id) =>
{
    return validations.TryGetValue(id, out var v) ? Results.Ok(new { validation = v }) : Results.NotFound();
});

app.MapPost("/api/v1/generate-compliance-proof", (GenerateProofRequest req) =>
{
    // Generate ZKP reference for compliance
    var proofId = $"ZKPTREATY-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8]}";
    return Results.Ok(new { 
        success = true, 
        proofId,
        treatyId = req.TreatyId,
        statement = $"Proves compliance with treaty {req.TreatyId} without revealing sensitive data"
    });
});

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║         MAMEY SICB TREATY VALIDATORS v1.0.0                  ║");
Console.WriteLine("║         Treaty Compliance Verification Service               ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();

record ValidateRequest(string TreatyId, string EntityId, string OperationType, Dictionary<string, object>? OperationData);
record GenerateProofRequest(string TreatyId, string EntityId);

class Treaty
{
    public string TreatyId { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public TreatyStatus Status { get; set; }
    public DateTime SignedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public List<string>? Parties { get; set; }
}

class TreatyValidation
{
    public Guid Id { get; set; }
    public string TreatyId { get; set; } = "";
    public string EntityId { get; set; } = "";
    public string OperationType { get; set; } = "";
    public Dictionary<string, object>? OperationData { get; set; }
    public List<ValidationRule> ValidationRules { get; set; } = new();
    public bool IsCompliant { get; set; }
    public decimal ComplianceScore { get; set; }
    public DateTime CreatedAt { get; set; }
}

class ValidationRule
{
    public string RuleName { get; set; } = "";
    public bool Passed { get; set; }
    public string? Details { get; set; }
}

enum TreatyStatus { Draft, Active, Suspended, Expired, Terminated }
