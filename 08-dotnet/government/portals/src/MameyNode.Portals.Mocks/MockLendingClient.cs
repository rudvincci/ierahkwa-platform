using Bogus;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public interface IMameyLendingClient
{
    Task<List<LoanInfo>> ListLoansAsync(string borrowerId, string? status = null, int limit = 50, int offset = 0);
    Task<LoanInfo?> GetLoanAsync(string loanId);
    Task<List<MicroloanInfo>> ListMicroloansAsync(string borrowerId, int limit = 50, int offset = 0);
    Task<MicroloanInfo?> GetMicroloanAsync(string loanId);
    Task<List<RepaymentInfo>> GetRepaymentHistoryAsync(string loanId, int limit = 50, int offset = 0);
    Task<List<CollateralInfo>> GetCollateralAsync(string loanId);
}

public class MockMameyLendingClient : IMameyLendingClient
{
    private readonly Faker _faker = new();
    private readonly List<LoanInfo> _loans = new();
    private readonly List<MicroloanInfo> _microloans = new();
    private readonly List<RepaymentInfo> _repayments = new();
    private readonly Dictionary<string, List<CollateralInfo>> _collateral = new();

    public MockMameyLendingClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        // Generate loans
        var loanFaker = new Faker<LoanInfo>()
            .RuleFor(l => l.LoanId, f => $"LOAN-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(l => l.BorrowerId, f => $"BORR-{f.Random.AlphaNumeric(8)}")
            .RuleFor(l => l.Amount, f => f.Finance.Amount(1000, 100000, 2).ToString("F2"))
            .RuleFor(l => l.Currency, "USD")
            .RuleFor(l => l.InterestRate, f => f.Random.Double(2.5, 15.0).ToString("F2"))
            .RuleFor(l => l.TermMonths, f => f.Random.Int(12, 360))
            .RuleFor(l => l.RemainingBalance, (f, l) => 
            {
                var amount = decimal.Parse(l.Amount);
                return f.Finance.Amount(0, amount, 2).ToString("F2");
            })
            .RuleFor(l => l.NextPaymentAmount, (f, l) => 
            {
                var amount = decimal.Parse(l.Amount);
                return (amount / l.TermMonths).ToString("F2");
            })
            .RuleFor(l => l.NextPaymentDate, f => f.Date.Soon(30))
            .RuleFor(l => l.Status, f => f.PickRandom<LoanStatus>())
            .RuleFor(l => l.CreatedAt, f => f.Date.Past(12))
            .RuleFor(l => l.ApprovedAt, (f, l) => l.Status != LoanStatus.Pending ? f.Date.Between(l.CreatedAt, DateTime.Now) : null);

        _loans.AddRange(loanFaker.Generate(30));

        // Generate microloans
        var microloanFaker = new Faker<MicroloanInfo>()
            .RuleFor(m => m.LoanId, f => $"MICRO-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(m => m.BorrowerId, f => $"BORR-{f.Random.AlphaNumeric(8)}")
            .RuleFor(m => m.Amount, f => f.Finance.Amount(50, 5000, 2).ToString("F2"))
            .RuleFor(m => m.Currency, "USD")
            .RuleFor(m => m.RemainingBalance, (f, m) => 
            {
                var amount = decimal.Parse(m.Amount);
                return f.Finance.Amount(0, amount, 2).ToString("F2");
            })
            .RuleFor(m => m.Status, f => f.PickRandom<MicroloanStatus>())
            .RuleFor(m => m.CreatedAt, f => f.Date.Past(6))
            .RuleFor(m => m.DueDate, f => f.Date.Future(6));

        _microloans.AddRange(microloanFaker.Generate(20));

        // Generate repayments
        foreach (var loan in _loans.Where(l => l.Status == LoanStatus.Active))
        {
            var repaymentFaker = new Faker<RepaymentInfo>()
                .RuleFor(r => r.RepaymentId, f => $"REPAY-{f.Random.AlphaNumeric(10).ToUpper()}")
                .RuleFor(r => r.LoanId, loan.LoanId)
                .RuleFor(r => r.Amount, loan.NextPaymentAmount)
                .RuleFor(r => r.Currency, loan.Currency)
                .RuleFor(r => r.PaymentDate, f => f.Date.Between(loan.CreatedAt, DateTime.Now))
                .RuleFor(r => r.Status, f => f.PickRandom<RepaymentStatus>());

            var repaymentCount = _faker.Random.Int(0, 12);
            _repayments.AddRange(repaymentFaker.Generate(repaymentCount));
        }

        // Generate collateral
        foreach (var loan in _loans.Take(10))
        {
            var collateralFaker = new Faker<CollateralInfo>()
                .RuleFor(c => c.CollateralId, f => $"COLL-{f.Random.AlphaNumeric(10).ToUpper()}")
                .RuleFor(c => c.CollateralType, f => f.PickRandom("Real Estate", "Vehicle", "Equipment", "Inventory", "Securities"))
                .RuleFor(c => c.Value, f => f.Finance.Amount(5000, 50000, 2).ToString("F2"))
                .RuleFor(c => c.Currency, "USD")
                .RuleFor(c => c.Description, f => f.Lorem.Sentence());

            var collateralCount = _faker.Random.Int(1, 3);
            _collateral[loan.LoanId] = collateralFaker.Generate(collateralCount);
        }
    }

    public Task<List<LoanInfo>> ListLoansAsync(string borrowerId, string? status = null, int limit = 50, int offset = 0)
    {
        var loans = _loans
            .Where(l => l.BorrowerId == borrowerId)
            .Where(l => status == null || l.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(l => l.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToList();
        return Task.FromResult(loans);
    }

    public Task<LoanInfo?> GetLoanAsync(string loanId)
    {
        return Task.FromResult(_loans.FirstOrDefault(l => l.LoanId == loanId));
    }

    public Task<List<MicroloanInfo>> ListMicroloansAsync(string borrowerId, int limit = 50, int offset = 0)
    {
        var microloans = _microloans
            .Where(m => m.BorrowerId == borrowerId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToList();
        return Task.FromResult(microloans);
    }

    public Task<MicroloanInfo?> GetMicroloanAsync(string loanId)
    {
        return Task.FromResult(_microloans.FirstOrDefault(m => m.LoanId == loanId));
    }

    public Task<List<RepaymentInfo>> GetRepaymentHistoryAsync(string loanId, int limit = 50, int offset = 0)
    {
        var repayments = _repayments
            .Where(r => r.LoanId == loanId)
            .OrderByDescending(r => r.PaymentDate)
            .Skip(offset)
            .Take(limit)
            .ToList();
        return Task.FromResult(repayments);
    }

    public Task<List<CollateralInfo>> GetCollateralAsync(string loanId)
    {
        var collateral = _collateral.TryGetValue(loanId, out var coll) ? coll : new List<CollateralInfo>();
        return Task.FromResult(collateral);
    }
}
