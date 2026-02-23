using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockFBDETBClient : IFBDETBClient
{
    private readonly Faker _faker = new();
    private readonly List<AccountInfo> _accounts = new();
    private readonly List<WalletInfo> _wallets = new();
    private readonly List<CardInfo> _cards = new();
    private readonly List<TerminalInfo> _terminals = new();
    private readonly List<LoanInfo> _loans = new();
    private readonly List<CreditRiskInfo> _creditRisks = new();
    private readonly List<FBDETBCollateralInfo> _collaterals = new();
    private readonly List<ExchangeInfo> _exchanges = new();
    private readonly List<TreasuryInfo> _treasuryInfo = new();
    private readonly List<ComplianceInfo> _complianceChecks = new();
    private readonly List<SecurityInfo> _securityAlerts = new();
    private readonly List<InsuranceInfo> _insurancePolicies = new();

    public MockFBDETBClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var accountFaker = new Faker<AccountInfo>()
            .RuleFor(a => a.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(a => a.BlockchainAccount, f => $"0x{f.Random.AlphaNumeric(40)}")
            .RuleFor(a => a.Balance, f => f.Finance.Amount(1000, 100000, 2).ToString("F2"))
            .RuleFor(a => a.Currency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(a => a.Status, f => f.PickRandom<AccountStatus>())
            .RuleFor(a => a.AccountType, f => f.PickRandom("Checking", "Savings", "Investment"))
            .RuleFor(a => a.CreatedAt, f => f.Date.Past(2))
            .RuleFor(a => a.OwnerId, f => $"OWNER-{f.Random.AlphaNumeric(8)}");

        _accounts.AddRange(accountFaker.Generate(100));

        var walletFaker = new Faker<WalletInfo>()
            .RuleFor(w => w.WalletId, f => $"WALLET-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(w => w.AccountId, f => _accounts[f.Random.Int(0, _accounts.Count - 1)].AccountId)
            .RuleFor(w => w.Balances, f => new Dictionary<string, string> { { "USD", f.Finance.Amount(100, 10000, 2).ToString("F2") }, { "EUR", f.Finance.Amount(50, 5000, 2).ToString("F2") } })
            .RuleFor(w => w.CreatedAt, f => f.Date.Past(1))
            .RuleFor(w => w.WalletType, "Standard")
            .RuleFor(w => w.IsActive, true);

        _wallets.AddRange(walletFaker.Generate(80));

        var cardFaker = new Faker<CardInfo>()
            .RuleFor(c => c.CardId, f => $"CARD-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.AccountId, f => _accounts[f.Random.Int(0, _accounts.Count - 1)].AccountId)
            .RuleFor(c => c.CardNumber, f => f.Finance.CreditCardNumber())
            .RuleFor(c => c.CardType, f => f.PickRandom("Virtual", "Physical", "Biometric"))
            .RuleFor(c => c.ExpiryDate, f => f.Date.Future(3))
            .RuleFor(c => c.IsActive, f => f.Random.Bool(0.9f))
            .RuleFor(c => c.IssuedAt, f => f.Date.Past(1))
            .RuleFor(c => c.Status, f => f.PickRandom("Active", "Suspended", "Expired"));

        _cards.AddRange(cardFaker.Generate(150));

        var terminalFaker = new Faker<TerminalInfo>()
            .RuleFor(t => t.TerminalId, f => $"TERM-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(t => t.MerchantId, f => $"MERCH-{f.Random.AlphaNumeric(8)}")
            .RuleFor(t => t.TerminalType, f => f.PickRandom("Mobile", "POS", "Kiosk"))
            .RuleFor(t => t.IsActive, f => f.Random.Bool(0.85f))
            .RuleFor(t => t.RegisteredAt, f => f.Date.Past(1))
            .RuleFor(t => t.Location, f => f.Address.City());

        _terminals.AddRange(terminalFaker.Generate(50));

        // Loans, risks, collaterals, etc. using similar patterns...
        var loanFaker = new Faker<LoanInfo>()
            .RuleFor(l => l.LoanId, f => $"LOAN-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(l => l.BorrowerId, f => $"BORR-{f.Random.AlphaNumeric(8)}")
            .RuleFor(l => l.Amount, f => f.Finance.Amount(5000, 500000, 2).ToString("F2"))
            .RuleFor(l => l.InterestRate, f => f.Random.Decimal(3.0m, 12.0m).ToString("F2"))
            .RuleFor(l => l.TermMonths, f => f.Random.Int(12, 60))
            .RuleFor(l => l.RemainingBalance, (f, l) => (decimal.Parse(l.Amount) * f.Random.Decimal(0.1m, 1.0m)).ToString("F2"))
            .RuleFor(l => l.NextPaymentAmount, (f, l) => (decimal.Parse(l.Amount) / l.TermMonths).ToString("F2"))
            .RuleFor(l => l.NextPaymentDate, (f, l) => f.Date.Between(l.CreatedAt, l.CreatedAt.AddMonths(1)))
            .RuleFor(l => l.Currency, "USD")
            .RuleFor(l => l.CreatedAt, f => f.Date.Past(1))
            .RuleFor(l => l.Status, f => f.PickRandom<LoanStatus>());

        _loans.AddRange(loanFaker.Generate(75));

        // Add remaining mock data generators...
        var riskFaker = new Faker<CreditRiskInfo>()
            .RuleFor(r => r.RiskId, f => $"RISK-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(r => r.AccountId, f => _accounts[f.Random.Int(0, _accounts.Count - 1)].AccountId)
            .RuleFor(r => r.RiskScore, f => f.Random.Double(300, 850).ToString("F0"))
            .RuleFor(r => r.RiskLevel, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(r => r.AssessedAt, f => f.Date.Recent(30))
            .RuleFor(r => r.RiskFactors, f => f.Lorem.Words(f.Random.Int(1, 4)).ToList())
            .RuleFor(r => r.Recommendation, f => f.Lorem.Sentence());

        _creditRisks.AddRange(riskFaker.Generate(60));

        var collateralFaker = new Faker<FBDETBCollateralInfo>()
            .RuleFor(c => c.CollateralId, f => $"COLL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.LoanId, f => _loans[f.Random.Int(0, _loans.Count - 1)].LoanId)
            .RuleFor(c => c.AssetType, f => f.PickRandom("RealEstate", "Vehicle", "Commodity", "Security"))
            .RuleFor(c => c.AssetValue, f => f.Finance.Amount(10000, 500000, 2).ToString("F2"))
            .RuleFor(c => c.Currency, "USD")
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(1))
            .RuleFor(c => c.Status, "Active");

        _collaterals.AddRange(collateralFaker.Generate(40));

        var exchangeFaker = new Faker<ExchangeInfo>()
            .RuleFor(e => e.ExchangeId, f => $"EXCH-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(e => e.FromCurrency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(e => e.ToCurrency, f => f.PickRandom("USD", "EUR", "GBP", "CAD"))
            .RuleFor(e => e.ExchangeRate, f => f.Random.Double(0.5, 2.0).ToString("F4"))
            .RuleFor(e => e.Amount, f => f.Finance.Amount(1000, 50000, 2).ToString("F2"))
            .RuleFor(e => e.ConvertedAmount, (f, e) => (decimal.Parse(e.Amount) * decimal.Parse(e.ExchangeRate)).ToString("F2"))
            .RuleFor(e => e.ExecutedAt, f => f.Date.Recent(30))
            .RuleFor(e => e.Status, "Completed");

        _exchanges.AddRange(exchangeFaker.Generate(100));

        var treasuryFaker = new Faker<TreasuryInfo>()
            .RuleFor(t => t.TreasuryId, f => $"TREAS-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(t => t.Currency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(t => t.TotalSupply, f => f.Finance.Amount(10000000, 1000000000, 2).ToString("F2"))
            .RuleFor(t => t.CirculatingSupply, (f, t) => (decimal.Parse(t.TotalSupply) * f.Random.Decimal(0.7m, 0.95m)).ToString("F2"))
            .RuleFor(t => t.TreasuryBalance, (f, t) => (decimal.Parse(t.TotalSupply) - decimal.Parse(t.CirculatingSupply)).ToString("F2"))
            .RuleFor(t => t.LastUpdated, f => f.Date.Recent(1));

        _treasuryInfo.AddRange(treasuryFaker.Generate(5));

        var complianceFaker = new Faker<ComplianceInfo>()
            .RuleFor(c => c.ComplianceId, f => $"COMP-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.EntityId, f => _accounts[f.Random.Int(0, _accounts.Count - 1)].AccountId)
            .RuleFor(c => c.EntityType, f => f.PickRandom("Account", "Transaction", "Loan"))
            .RuleFor(c => c.ComplianceType, f => f.PickRandom("KYC", "AML", "Sanctions"))
            .RuleFor(c => c.Status, f => f.PickRandom("Pending", "Verified", "Rejected"))
            .RuleFor(c => c.CheckedAt, f => f.Date.Recent(60))
            .RuleFor(c => c.IsCompliant, f => f.Random.Bool(0.9f))
            .RuleFor(c => c.Violations, f => f.Random.Bool(0.1f) ? f.Lorem.Words(f.Random.Int(1, 2)).ToList() : new List<string>());

        _complianceChecks.AddRange(complianceFaker.Generate(120));

        var securityFaker = new Faker<SecurityInfo>()
            .RuleFor(s => s.SecurityId, f => $"SEC-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(s => s.AccountId, f => _accounts[f.Random.Int(0, _accounts.Count - 1)].AccountId)
            .RuleFor(s => s.SecurityType, f => f.PickRandom("Fraud", "Suspicious", "Unauthorized", "Breach"))
            .RuleFor(s => s.Severity, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(s => s.Status, f => f.PickRandom("Open", "Investigating", "Resolved", "FalsePositive"))
            .RuleFor(s => s.DetectedAt, f => f.Date.Recent(7))
            .RuleFor(s => s.Description, f => f.Lorem.Sentence());

        _securityAlerts.AddRange(securityFaker.Generate(30));

        var insuranceFaker = new Faker<InsuranceInfo>()
            .RuleFor(i => i.InsuranceId, f => $"INS-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(i => i.PolicyType, f => f.PickRandom("ClanMutual", "Sovereign", "Commercial"))
            .RuleFor(i => i.CoverageAmount, f => f.Finance.Amount(50000, 1000000, 2).ToString("F2"))
            .RuleFor(i => i.Premium, (f, i) => (decimal.Parse(i.CoverageAmount) * 0.01m).ToString("F2"))
            .RuleFor(i => i.Currency, "USD")
            .RuleFor(i => i.EffectiveDate, f => f.Date.Past(1))
            .RuleFor(i => i.ExpiryDate, (f, i) => f.Date.Between(i.EffectiveDate, i.EffectiveDate.AddYears(1)))
            .RuleFor(i => i.Status, f => f.PickRandom("Active", "Expired", "Cancelled"))
            .RuleFor(i => i.InsuredId, f => $"INSURED-{f.Random.AlphaNumeric(8)}");

        _insurancePolicies.AddRange(insuranceFaker.Generate(25));
    }

    public Task<List<AccountInfo>> GetAccountsAsync() => Task.FromResult(_accounts);
    public Task<AccountInfo?> GetAccountAsync(string accountId) => Task.FromResult(_accounts.FirstOrDefault(a => a.AccountId == accountId));
    public Task<List<WalletInfo>> GetWalletsAsync() => Task.FromResult(_wallets);
    public Task<List<CardInfo>> GetCardsAsync() => Task.FromResult(_cards);
    public Task<List<TerminalInfo>> GetTerminalsAsync() => Task.FromResult(_terminals);
    public Task<List<LoanInfo>> GetLoansAsync() => Task.FromResult(_loans);
    public Task<List<CreditRiskInfo>> GetCreditRisksAsync() => Task.FromResult(_creditRisks);
    public Task<List<FBDETBCollateralInfo>> GetCollateralsAsync() => Task.FromResult(_collaterals);
    public Task<List<ExchangeInfo>> GetExchangesAsync() => Task.FromResult(_exchanges);
    public Task<List<TreasuryInfo>> GetTreasuryInfoAsync() => Task.FromResult(_treasuryInfo);
    public Task<List<ComplianceInfo>> GetComplianceChecksAsync() => Task.FromResult(_complianceChecks);
    public Task<List<SecurityInfo>> GetSecurityAlertsAsync() => Task.FromResult(_securityAlerts);
    public Task<List<InsuranceInfo>> GetInsurancePoliciesAsync() => Task.FromResult(_insurancePolicies);
}

