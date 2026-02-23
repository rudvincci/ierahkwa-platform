using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockSICBClient : ISICBClient
{
    private readonly Faker _faker = new();
    private readonly List<MonetaryInstrumentInfo> _monetaryInstruments = new();
    private readonly List<LedgerReserveInfo> _ledgerReserves = new();
    private readonly List<LendingInfo> _lendingOperations = new();
    private readonly List<MonetaryPolicyInfo> _monetaryPolicies = new();
    private readonly List<FiscalOperationInfo> _fiscalOperations = new();
    private readonly List<TreasuryProgramInfo> _treasuryPrograms = new();
    private readonly List<ForeignExchangeInfo> _foreignExchanges = new();
    private readonly List<ComplianceEnforcementInfo> _complianceEnforcements = new();
    private readonly List<CitizenToolInfo> _citizenTools = new();
    private readonly List<SystemIntegrityInfo> _systemIntegrityChecks = new();
    private readonly List<TreasuryInstrumentInfo> _treasuryInstruments = new();

    public MockSICBClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var instrumentFaker = new Faker<MonetaryInstrumentInfo>()
            .RuleFor(i => i.InstrumentId, f => $"INST-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(i => i.Type, f => f.PickRandom<MonetaryInstrumentType>())
            .RuleFor(i => i.Currency, f => f.PickRandom("USD", "EUR", "GBP", "CAD", "AUD"))
            .RuleFor(i => i.TotalSupply, f => f.Finance.Amount(1000000, 100000000, 2).ToString("F2"))
            .RuleFor(i => i.CirculatingSupply, (f, i) => (decimal.Parse(i.TotalSupply) * f.Random.Decimal(0.6m, 0.95m)).ToString("F2"))
            .RuleFor(i => i.IssuedAt, f => f.Date.Past(5))
            .RuleFor(i => i.Issuer, "SICB")
            .RuleFor(i => i.Status, f => f.PickRandom("Active", "Suspended", "Retired"));

        _monetaryInstruments.AddRange(instrumentFaker.Generate(20));

        var reserveFaker = new Faker<LedgerReserveInfo>()
            .RuleFor(r => r.ReserveId, f => $"RES-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(r => r.Currency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(r => r.TotalReserves, f => f.Finance.Amount(50000000, 500000000, 2).ToString("F2"))
            .RuleFor(r => r.RequiredReserves, (f, r) => (decimal.Parse(r.TotalReserves) * f.Random.Decimal(0.1m, 0.3m)).ToString("F2"))
            .RuleFor(r => r.ReserveRatio, (f, r) => (decimal.Parse(r.RequiredReserves) / decimal.Parse(r.TotalReserves) * 100).ToString("F2"))
            .RuleFor(r => r.Status, f => f.PickRandom<ReserveStatus>())
            .RuleFor(r => r.LastUpdated, f => f.Date.Recent(7))
            .RuleFor(r => r.BackingAssets, f => f.PickRandom("Gold", "ForeignCurrency", "GovernmentBonds", "Mixed"));

        _ledgerReserves.AddRange(reserveFaker.Generate(10));

        var lendingFaker = new Faker<LendingInfo>()
            .RuleFor(l => l.LoanId, f => $"LOAN-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(l => l.BorrowerId, f => $"BORR-{f.Random.AlphaNumeric(8)}")
            .RuleFor(l => l.Amount, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(l => l.Currency, "USD")
            .RuleFor(l => l.InterestRate, f => f.Random.Decimal(2.0m, 8.0m).ToString("F2"))
            .RuleFor(l => l.Status, f => f.PickRandom("Active", "Completed", "Defaulted"))
            .RuleFor(l => l.CreatedAt, f => f.Date.Past(2))
            .RuleFor(l => l.MaturityDate, (f, l) => f.Date.Between(l.CreatedAt, l.CreatedAt.AddYears(5)))
            .RuleFor(l => l.RemainingBalance, (f, l) => f.Random.Bool(0.3f) ? "0" : (decimal.Parse(l.Amount) * f.Random.Decimal(0.1m, 0.9m)).ToString("F2"));

        _lendingOperations.AddRange(lendingFaker.Generate(100));

        var policyFaker = new Faker<MonetaryPolicyInfo>()
            .RuleFor(p => p.PolicyId, f => $"POL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.PolicyType, f => f.PickRandom("InterestRate", "ReserveRequirement", "QuantitativeEasing", "CurrencyPeg"))
            .RuleFor(p => p.InterestRate, f => f.Random.Decimal(0.5m, 5.0m).ToString("F2"))
            .RuleFor(p => p.ReserveRequirement, f => f.Random.Decimal(5.0m, 20.0m).ToString("F2"))
            .RuleFor(p => p.EffectiveDate, f => f.Date.Recent(90))
            .RuleFor(p => p.Status, f => f.PickRandom("Active", "Pending", "Expired"))
            .RuleFor(p => p.Description, f => f.Lorem.Sentence());

        _monetaryPolicies.AddRange(policyFaker.Generate(15));

        var fiscalFaker = new Faker<FiscalOperationInfo>()
            .RuleFor(f => f.OperationId, f => $"FISC-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(f => f.OperationType, f => f.PickRandom("Expenditure", "Revenue", "Transfer", "Investment"))
            .RuleFor(f => f.Amount, f => f.Finance.Amount(50000, 5000000, 2).ToString("F2"))
            .RuleFor(f => f.Currency, "USD")
            .RuleFor(f => f.ProgramId, f => $"PROG-{f.Random.AlphaNumeric(8)}")
            .RuleFor(f => f.CreatedAt, f => f.Date.Recent(60))
            .RuleFor(f => f.Status, f => f.PickRandom("Pending", "Approved", "Executed", "Cancelled"))
            .RuleFor(f => f.RecipientId, f => $"RECIP-{f.Random.AlphaNumeric(8)}");

        _fiscalOperations.AddRange(fiscalFaker.Generate(200));

        var programFaker = new Faker<TreasuryProgramInfo>()
            .RuleFor(p => p.ProgramId, f => $"PROG-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(p => p.ProgramName, f => f.Company.CatchPhrase())
            .RuleFor(p => p.Budget, f => f.Finance.Amount(1000000, 50000000, 2).ToString("F2"))
            .RuleFor(p => p.Allocated, (f, p) => (decimal.Parse(p.Budget) * f.Random.Decimal(0.3m, 0.8m)).ToString("F2"))
            .RuleFor(p => p.Disbursed, (f, p) => (decimal.Parse(p.Allocated) * f.Random.Decimal(0.5m, 0.95m)).ToString("F2"))
            .RuleFor(p => p.Currency, "USD")
            .RuleFor(p => p.StartDate, f => f.Date.Past(2))
            .RuleFor(p => p.EndDate, (f, p) => f.Date.Between(p.StartDate, p.StartDate.AddYears(3)))
            .RuleFor(p => p.Status, f => f.PickRandom("Active", "Completed", "Suspended"));

        _treasuryPrograms.AddRange(programFaker.Generate(25));

        var fxFaker = new Faker<ForeignExchangeInfo>()
            .RuleFor(f => f.ExchangeId, f => $"FX-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(f => f.FromCurrency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(f => f.ToCurrency, f => f.PickRandom("USD", "EUR", "GBP", "CAD", "AUD", "JPY"))
            .RuleFor(f => f.ExchangeRate, f => f.Random.Double(0.5, 2.0).ToString("F4"))
            .RuleFor(f => f.Amount, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(f => f.ConvertedAmount, (f, e) => (decimal.Parse(e.Amount) * decimal.Parse(e.ExchangeRate)).ToString("F2"))
            .RuleFor(f => f.ExecutedAt, f => f.Date.Recent(30))
            .RuleFor(f => f.SettlementId, f => $"SETTLE-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(f => f.Status, "Completed");

        _foreignExchanges.AddRange(fxFaker.Generate(150));

        var complianceFaker = new Faker<ComplianceEnforcementInfo>()
            .RuleFor(c => c.EnforcementId, f => $"ENF-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.EntityId, f => $"ENT-{f.Random.AlphaNumeric(8)}")
            .RuleFor(c => c.EntityType, f => f.PickRandom("Bank", "Account", "Transaction", "Program"))
            .RuleFor(c => c.ViolationType, f => f.PickRandom("NonCompliance", "Fraud", "MoneyLaundering", "Sanctions"))
            .RuleFor(c => c.Severity, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(c => c.Status, f => f.PickRandom("Open", "UnderReview", "Resolved", "Dismissed"))
            .RuleFor(c => c.ReportedAt, f => f.Date.Recent(90))
            .RuleFor(c => c.Description, f => f.Lorem.Sentence());

        _complianceEnforcements.AddRange(complianceFaker.Generate(50));

        var toolFaker = new Faker<CitizenToolInfo>()
            .RuleFor(t => t.ToolId, f => $"TOOL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(t => t.ToolName, f => f.Commerce.ProductName())
            .RuleFor(t => t.ToolType, f => f.PickRandom("Calculator", "Simulator", "Analyzer", "Reporter"))
            .RuleFor(t => t.Description, f => f.Lorem.Sentence())
            .RuleFor(t => t.IsActive, f => f.Random.Bool(0.9f))
            .RuleFor(t => t.CreatedAt, f => f.Date.Past(1))
            .RuleFor(t => t.UsageCount, f => f.Random.Int(0, 10000));

        _citizenTools.AddRange(toolFaker.Generate(15));

        var integrityFaker = new Faker<SystemIntegrityInfo>()
            .RuleFor(s => s.CheckId, f => $"CHECK-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(s => s.CheckType, f => f.PickRandom("Security", "Performance", "Compliance", "Availability"))
            .RuleFor(s => s.IsHealthy, f => f.Random.Bool(0.85f))
            .RuleFor(s => s.HealthScore, f => f.Random.Int(70, 100).ToString())
            .RuleFor(s => s.CheckedAt, f => f.Date.Recent(1))
            .RuleFor(s => s.Issues, (f, s) => s.IsHealthy ? new List<string>() : f.Lorem.Words(f.Random.Int(1, 3)).ToList())
            .RuleFor(s => s.Status, (f, s) => s.IsHealthy ? "OK" : "Warning");

        _systemIntegrityChecks.AddRange(integrityFaker.Generate(20));

        var treasuryFaker = new Faker<TreasuryInstrumentInfo>()
            .RuleFor(t => t.InstrumentId, f => $"TINST-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(t => t.InstrumentType, f => f.PickRandom("Bond", "TreasuryBill", "TreasuryNote", "TreasuryBond"))
            .RuleFor(t => t.FaceValue, f => f.Finance.Amount(1000, 100000, 2).ToString("F2"))
            .RuleFor(t => t.Currency, "USD")
            .RuleFor(t => t.InterestRate, f => f.Random.Decimal(1.0m, 5.0m).ToString("F2"))
            .RuleFor(t => t.IssueDate, f => f.Date.Past(2))
            .RuleFor(t => t.MaturityDate, (f, t) => f.Date.Between(t.IssueDate, t.IssueDate.AddYears(10)))
            .RuleFor(t => t.Status, f => f.PickRandom("Active", "Matured", "Redeemed"));

        _treasuryInstruments.AddRange(treasuryFaker.Generate(30));
    }

    public Task<List<MonetaryInstrumentInfo>> GetMonetaryInstrumentsAsync() => Task.FromResult(_monetaryInstruments);
    public Task<List<LedgerReserveInfo>> GetLedgerReservesAsync() => Task.FromResult(_ledgerReserves);
    public Task<List<LendingInfo>> GetLendingOperationsAsync() => Task.FromResult(_lendingOperations);
    public Task<List<MonetaryPolicyInfo>> GetMonetaryPoliciesAsync() => Task.FromResult(_monetaryPolicies);
    public Task<List<FiscalOperationInfo>> GetFiscalOperationsAsync() => Task.FromResult(_fiscalOperations);
    public Task<List<TreasuryProgramInfo>> GetTreasuryProgramsAsync() => Task.FromResult(_treasuryPrograms);
    public Task<List<ForeignExchangeInfo>> GetForeignExchangesAsync() => Task.FromResult(_foreignExchanges);
    public Task<List<ComplianceEnforcementInfo>> GetComplianceEnforcementsAsync() => Task.FromResult(_complianceEnforcements);
    public Task<List<CitizenToolInfo>> GetCitizenToolsAsync() => Task.FromResult(_citizenTools);
    public Task<List<SystemIntegrityInfo>> GetSystemIntegrityChecksAsync() => Task.FromResult(_systemIntegrityChecks);
    public Task<List<TreasuryInstrumentInfo>> GetTreasuryInstrumentsAsync() => Task.FromResult(_treasuryInstruments);
}

