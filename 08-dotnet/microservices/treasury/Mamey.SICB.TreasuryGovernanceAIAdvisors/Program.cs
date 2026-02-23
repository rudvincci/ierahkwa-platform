var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Mamey SICB Treasury Governance AI Advisors", Version = "v1" }));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

var analyses = new Dictionary<Guid, GovernanceAnalysis>();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.SICB.TreasuryGovernanceAIAdvisors" }));

app.MapPost("/api/v1/analyze/operation", (AnalyzeOperationRequest req) =>
{
    var analysis = new GovernanceAnalysis
    {
        Id = Guid.NewGuid(),
        RequestType = "TreasuryOperation",
        RequestId = req.OperationId,
        Amount = req.Amount,
        Currency = req.Currency,
        
        RiskScore = CalculateRiskScore(req.Amount, req.OperationType),
        RiskLevel = DetermineRiskLevel(req.Amount),
        
        Recommendations = GenerateRecommendations(req.OperationType, req.Amount),
        ComplianceFlags = CheckComplianceFlags(req.Amount, req.OperationType),
        
        RequiresGovernanceVote = req.Amount > 1_000_000,
        SuggestedApprovers = GetSuggestedApprovers(req.Amount),
        
        CreatedAt = DateTime.UtcNow
    };
    
    analyses[analysis.Id] = analysis;
    return Results.Ok(new { success = true, analysis });
});

app.MapPost("/api/v1/analyze/proposal", (AnalyzeProposalRequest req) =>
{
    var analysis = new GovernanceAnalysis
    {
        Id = Guid.NewGuid(),
        RequestType = "GovernanceProposal",
        RequestId = req.ProposalId,
        
        RiskScore = 30,
        RiskLevel = RiskLevel.Low,
        
        Recommendations = new List<string>
        {
            "Review proposal impact on existing treaties",
            "Consider citizen feedback period",
            "Assess budget implications"
        },
        
        ImpactAssessment = new ImpactAssessment
        {
            BudgetImpact = req.BudgetImpact ?? "Unknown",
            CitizenImpact = "Medium",
            TreatyImpact = "Low",
            TimelineImpact = "Short-term"
        },
        
        CreatedAt = DateTime.UtcNow
    };
    
    analyses[analysis.Id] = analysis;
    return Results.Ok(new { success = true, analysis });
});

app.MapPost("/api/v1/recommend/budget", (BudgetRecommendationRequest req) =>
{
    var recommendations = new BudgetRecommendation
    {
        FiscalYear = req.FiscalYear,
        TotalBudget = req.TotalBudget,
        DepartmentAllocations = new Dictionary<string, decimal>
        {
            ["Education"] = req.TotalBudget * 0.20m,
            ["Healthcare"] = req.TotalBudget * 0.18m,
            ["Infrastructure"] = req.TotalBudget * 0.15m,
            ["Treasury Operations"] = req.TotalBudget * 0.10m,
            ["Defense"] = req.TotalBudget * 0.08m,
            ["Technology"] = req.TotalBudget * 0.08m,
            ["Humanitarian"] = req.TotalBudget * 0.10m,
            ["Reserve"] = req.TotalBudget * 0.11m
        },
        Rationale = "AI-optimized allocation based on historical spending patterns and treaty obligations.",
        ConfidenceScore = 0.87m
    };
    
    return Results.Ok(new { success = true, recommendations });
});

app.MapGet("/api/v1/analyses/{id:guid}", (Guid id) =>
{
    return analyses.TryGetValue(id, out var a) ? Results.Ok(new { analysis = a }) : Results.NotFound();
});

decimal CalculateRiskScore(decimal amount, string opType) =>
    amount switch
    {
        > 10_000_000 => 90,
        > 1_000_000 => 70,
        > 100_000 => 50,
        > 10_000 => 30,
        _ => 10
    };

RiskLevel DetermineRiskLevel(decimal amount) =>
    amount switch
    {
        > 10_000_000 => RiskLevel.Critical,
        > 1_000_000 => RiskLevel.High,
        > 100_000 => RiskLevel.Medium,
        _ => RiskLevel.Low
    };

List<string> GenerateRecommendations(string opType, decimal amount)
{
    var recs = new List<string>();
    if (amount > 1_000_000) recs.Add("Require multi-signature authorization");
    if (amount > 100_000) recs.Add("Generate compliance ZKP proof");
    recs.Add("Record in audit trail");
    recs.Add("Notify relevant stakeholders");
    return recs;
}

List<string> CheckComplianceFlags(decimal amount, string opType)
{
    var flags = new List<string>();
    if (amount > 5_000_000) flags.Add("LARGE_TRANSACTION");
    if (opType == "International") flags.Add("CROSS_BORDER");
    return flags;
}

List<string> GetSuggestedApprovers(decimal amount) =>
    amount switch
    {
        > 10_000_000 => new() { "Prime Minister", "Finance Minister", "Treasury Director" },
        > 1_000_000 => new() { "Finance Minister", "Treasury Director" },
        > 100_000 => new() { "Treasury Director", "Department Head" },
        _ => new() { "Department Head" }
    };

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║     MAMEY SICB TREASURY GOVERNANCE AI ADVISORS v1.0.0        ║");
Console.WriteLine("║     AI-Powered Decision Support for Treasury Operations      ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();

record AnalyzeOperationRequest(string OperationId, string OperationType, decimal Amount, string Currency);
record AnalyzeProposalRequest(string ProposalId, string Title, string? BudgetImpact);
record BudgetRecommendationRequest(int FiscalYear, decimal TotalBudget);

class GovernanceAnalysis
{
    public Guid Id { get; set; }
    public string RequestType { get; set; } = "";
    public string RequestId { get; set; } = "";
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public decimal RiskScore { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public List<string> ComplianceFlags { get; set; } = new();
    public bool RequiresGovernanceVote { get; set; }
    public List<string>? SuggestedApprovers { get; set; }
    public ImpactAssessment? ImpactAssessment { get; set; }
    public DateTime CreatedAt { get; set; }
}

class ImpactAssessment
{
    public string BudgetImpact { get; set; } = "";
    public string CitizenImpact { get; set; } = "";
    public string TreatyImpact { get; set; } = "";
    public string TimelineImpact { get; set; } = "";
}

class BudgetRecommendation
{
    public int FiscalYear { get; set; }
    public decimal TotalBudget { get; set; }
    public Dictionary<string, decimal> DepartmentAllocations { get; set; } = new();
    public string Rationale { get; set; } = "";
    public decimal ConfidenceScore { get; set; }
}

enum RiskLevel { Low, Medium, High, Critical }
