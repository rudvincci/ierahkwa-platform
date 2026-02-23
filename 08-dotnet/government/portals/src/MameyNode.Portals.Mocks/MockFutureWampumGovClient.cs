using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockFutureWampumGovClient : IFutureWampumGovClient
{
    private readonly Faker _faker = new();
    private readonly List<FutureWampumGovDisbursementInfo> _disbursements = new();
    private readonly List<DisbursementBatchInfo> _batches = new();
    private readonly List<UBIProgramInfo> _ubiPrograms = new();
    private readonly List<UBIRecipientInfo> _ubiRecipients = new();
    private readonly List<BudgetAllocationInfo> _budgetAllocations = new();
    private readonly List<ProgramInfo> _programs = new();

    public MockFutureWampumGovClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var disbursementFaker = new Faker<FutureWampumGovDisbursementInfo>()
            .RuleFor(d => d.DisbursementId, f => $"DISB-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(d => d.ProgramId, f => $"PROG-{f.Random.AlphaNumeric(8)}")
            .RuleFor(d => d.RecipientId, f => $"RECIP-{f.Random.AlphaNumeric(8)}")
            .RuleFor(d => d.Amount, f => f.Finance.Amount(500, 5000, 2).ToString("F2"))
            .RuleFor(d => d.Currency, "USD")
            .RuleFor(d => d.Purpose, f => f.Lorem.Sentence())
            .RuleFor(d => d.Status, f => f.PickRandom<PaymentStatus>())
            .RuleFor(d => d.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(d => d.ProcessedAt, (f, d) => d.Status == PaymentStatus.Completed ? f.Date.Between(d.CreatedAt, DateTime.Now) : null)
            .RuleFor(d => d.BatchId, f => $"BATCH-{f.Random.AlphaNumeric(8)}");

        _disbursements.AddRange(disbursementFaker.Generate(500));

        var batchFaker = new Faker<DisbursementBatchInfo>()
            .RuleFor(b => b.BatchId, f => $"BATCH-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(b => b.ProgramId, f => $"PROG-{f.Random.AlphaNumeric(8)}")
            .RuleFor(b => b.TotalRecipients, f => f.Random.Int(100, 10000))
            .RuleFor(b => b.ProcessedRecipients, (f, b) => f.Random.Int(0, b.TotalRecipients))
            .RuleFor(b => b.TotalAmount, f => f.Finance.Amount(50000, 5000000, 2).ToString("F2"))
            .RuleFor(b => b.Currency, "USD")
            .RuleFor(b => b.Status, f => f.PickRandom("Pending", "Processing", "Completed", "Failed"))
            .RuleFor(b => b.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(b => b.CompletedAt, (f, b) => b.Status == "Completed" ? f.Date.Between(b.CreatedAt, DateTime.Now) : null);

        _batches.AddRange(batchFaker.Generate(20));

        var ubiFaker = new Faker<UBIProgramInfo>()
            .RuleFor(u => u.ProgramId, f => $"UBI-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(u => u.ProgramName, f => $"Universal Basic Income - {f.Address.City()}")
            .RuleFor(u => u.MonthlyAmount, f => f.Finance.Amount(1000, 2000, 2).ToString("F2"))
            .RuleFor(u => u.Currency, "USD")
            .RuleFor(u => u.TotalRecipients, f => f.Random.Int(1000, 50000))
            .RuleFor(u => u.ActiveRecipients, (f, u) => f.Random.Int((int)(u.TotalRecipients * 0.8), u.TotalRecipients))
            .RuleFor(u => u.StartDate, f => f.Date.Past(1))
            .RuleFor(u => u.EndDate, (f, u) => f.Random.Bool(0.3f) ? f.Date.Between(u.StartDate, u.StartDate.AddYears(2)) : null)
            .RuleFor(u => u.Status, f => f.PickRandom("Active", "Paused", "Completed"))
            .RuleFor(u => u.Budget, (f, u) => (decimal.Parse(u.MonthlyAmount) * u.TotalRecipients * 12).ToString("F2"))
            .RuleFor(u => u.Disbursed, (f, u) => (decimal.Parse(u.Budget) * f.Random.Decimal(0.2m, 0.8m)).ToString("F2"));

        _ubiPrograms.AddRange(ubiFaker.Generate(5));

        var recipientFaker = new Faker<UBIRecipientInfo>()
            .RuleFor(r => r.RecipientId, f => $"RECIP-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(r => r.ProgramId, f => _ubiPrograms[f.Random.Int(0, _ubiPrograms.Count - 1)].ProgramId)
            .RuleFor(r => r.CitizenId, f => $"CIT-{f.Random.AlphaNumeric(8)}")
            .RuleFor(r => r.MonthlyAmount, f => f.Finance.Amount(1000, 2000, 2).ToString("F2"))
            .RuleFor(r => r.Currency, "USD")
            .RuleFor(r => r.EnrollmentDate, f => f.Date.Past(1))
            .RuleFor(r => r.Status, f => f.PickRandom("Active", "Suspended", "Terminated"))
            .RuleFor(r => r.LastDisbursement, f => f.Date.Recent(30))
            .RuleFor(r => r.TotalDisbursements, f => f.Random.Int(1, 24));

        _ubiRecipients.AddRange(recipientFaker.Generate(1000));

        var budgetFaker = new Faker<BudgetAllocationInfo>()
            .RuleFor(b => b.AllocationId, f => $"ALLOC-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(b => b.MinistryId, f => $"MIN-{f.Random.AlphaNumeric(6)}")
            .RuleFor(b => b.MinistryName, f => f.PickRandom("Health", "Education", "Infrastructure", "Social Services", "Defense"))
            .RuleFor(b => b.AllocatedAmount, f => f.Finance.Amount(1000000, 50000000, 2).ToString("F2"))
            .RuleFor(b => b.SpentAmount, (f, b) => (decimal.Parse(b.AllocatedAmount) * f.Random.Decimal(0.1m, 0.7m)).ToString("F2"))
            .RuleFor(b => b.RemainingAmount, (f, b) => (decimal.Parse(b.AllocatedAmount) - decimal.Parse(b.SpentAmount)).ToString("F2"))
            .RuleFor(b => b.Currency, "USD")
            .RuleFor(b => b.FiscalYear, f => $"FY{f.Date.Past(1).Year}")
            .RuleFor(b => b.CreatedAt, f => f.Date.Past(1))
            .RuleFor(b => b.Status, f => f.PickRandom("Active", "Closed", "Overspent"));

        _budgetAllocations.AddRange(budgetFaker.Generate(15));

        var programFaker = new Faker<ProgramInfo>()
            .RuleFor(p => p.ProgramId, f => $"PROG-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(p => p.ProgramName, f => f.Company.CatchPhrase())
            .RuleFor(p => p.ProgramType, f => f.PickRandom("Welfare", "Education", "Healthcare", "Infrastructure", "Employment"))
            .RuleFor(p => p.Budget, f => f.Finance.Amount(500000, 10000000, 2).ToString("F2"))
            .RuleFor(p => p.Allocated, (f, p) => (decimal.Parse(p.Budget) * f.Random.Decimal(0.5m, 0.9m)).ToString("F2"))
            .RuleFor(p => p.Disbursed, (f, p) => (decimal.Parse(p.Allocated) * f.Random.Decimal(0.3m, 0.8m)).ToString("F2"))
            .RuleFor(p => p.Currency, "USD")
            .RuleFor(p => p.TotalRecipients, f => f.Random.Int(100, 5000))
            .RuleFor(p => p.StartDate, f => f.Date.Past(1))
            .RuleFor(p => p.EndDate, (f, p) => f.Date.Between(p.StartDate, p.StartDate.AddYears(3)))
            .RuleFor(p => p.Status, f => f.PickRandom("Active", "Completed", "Suspended"))
            .RuleFor(p => p.MinistryId, f => $"MIN-{f.Random.AlphaNumeric(6)}");

        _programs.AddRange(programFaker.Generate(30));
    }

    public Task<List<FutureWampumGovDisbursementInfo>> GetDisbursementsAsync() => Task.FromResult(_disbursements);
    public Task<List<DisbursementBatchInfo>> GetDisbursementBatchesAsync() => Task.FromResult(_batches);
    public Task<List<UBIProgramInfo>> GetUBIProgramsAsync() => Task.FromResult(_ubiPrograms);
    public Task<List<UBIRecipientInfo>> GetUBIRecipientsAsync() => Task.FromResult(_ubiRecipients);
    public Task<List<BudgetAllocationInfo>> GetBudgetAllocationsAsync() => Task.FromResult(_budgetAllocations);
    public Task<List<ProgramInfo>> GetProgramsAsync() => Task.FromResult(_programs);
    public Task<TransparencyDashboardData> GetTransparencyDashboardAsync()
    {
        var dashboard = new TransparencyDashboardData
        {
            TotalBudget = _budgetAllocations.Sum(b => decimal.Parse(b.AllocatedAmount)).ToString("F2"),
            TotalDisbursed = _disbursements.Where(d => d.Status == PaymentStatus.Completed).Sum(d => decimal.Parse(d.Amount)).ToString("F2"),
            TotalPrograms = _programs.Count.ToString(),
            ActivePrograms = _programs.Count(p => p.Status == "Active").ToString(),
            TotalRecipients = _ubiRecipients.Count,
            MinistryBudgets = _budgetAllocations.ToDictionary(b => b.MinistryName, b => b.AllocatedAmount),
            ProgramRecipients = _programs.ToDictionary(p => p.ProgramName, p => p.TotalRecipients),
            LastUpdated = DateTime.Now
        };
        return Task.FromResult(dashboard);
    }
}

