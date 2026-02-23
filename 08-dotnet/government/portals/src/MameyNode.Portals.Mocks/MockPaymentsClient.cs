using Bogus;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public interface IMameyPaymentsClient
{
    Task<List<PaymentInfo>> ListP2PPaymentsAsync(string accountId, int limit = 50, int offset = 0);
    Task<PaymentInfo?> GetP2PPaymentAsync(string paymentId);
    Task<List<MerchantPaymentInfo>> ListMerchantPaymentsAsync(string merchantId, int limit = 50, int offset = 0);
    Task<MerchantPaymentInfo?> GetMerchantPaymentAsync(string paymentId);
    Task<List<DisbursementInfo>> ListDisbursementsAsync(string programId, int limit = 50, int offset = 0);
    Task<DisbursementInfo?> GetDisbursementAsync(string disbursementId);
    Task<List<RecurringPaymentInfo>> ListRecurringPaymentsAsync(string accountId, int limit = 50, int offset = 0);
    Task<List<MultisigPaymentInfo>> ListMultisigPaymentsAsync(string accountId, int limit = 50, int offset = 0);
    Task<MultisigPaymentInfo?> GetMultisigPaymentAsync(string paymentId);
}

public class MockMameyPaymentsClient : IMameyPaymentsClient
{
    private readonly Faker _faker = new();
    private readonly List<PaymentInfo> _p2pPayments = new();
    private readonly List<MerchantPaymentInfo> _merchantPayments = new();
    private readonly List<DisbursementInfo> _disbursements = new();
    private readonly List<RecurringPaymentInfo> _recurringPayments = new();
    private readonly List<MultisigPaymentInfo> _multisigPayments = new();

    public MockMameyPaymentsClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        // Generate P2P payments
        var p2pFaker = new Faker<PaymentInfo>()
            .RuleFor(p => p.PaymentId, f => $"PAY-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.FromAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.Amount, f => f.Finance.Amount(10, 10000, 2).ToString("F2"))
            .RuleFor(p => p.Currency, "USD")
            .RuleFor(p => p.Status, f => f.PickRandom<PaymentStatus>())
            .RuleFor(p => p.TransactionId, f => $"TXN-{f.Random.AlphaNumeric(12).ToUpper()}")
            .RuleFor(p => p.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(p => p.CompletedAt, (f, p) => p.Status == PaymentStatus.Completed ? f.Date.Between(p.CreatedAt, DateTime.Now) : null)
            .RuleFor(p => p.Memo, f => f.Lorem.Sentence());

        _p2pPayments.AddRange(p2pFaker.Generate(50));

        // Generate merchant payments
        var merchantFaker = new Faker<MerchantPaymentInfo>()
            .RuleFor(p => p.PaymentId, f => $"MERCH-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.MerchantId, f => $"MERCH-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.CustomerAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.Amount, f => f.Finance.Amount(5, 500, 2).ToString("F2"))
            .RuleFor(p => p.Currency, "USD")
            .RuleFor(p => p.OrderId, f => $"ORD-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.Status, f => f.PickRandom<PaymentStatus>())
            .RuleFor(p => p.CreatedAt, f => f.Date.Recent(30));

        _merchantPayments.AddRange(merchantFaker.Generate(75));

        // Generate disbursements
        var disbursementFaker = new Faker<DisbursementInfo>()
            .RuleFor(d => d.DisbursementId, f => $"DISB-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(d => d.ProgramId, f => $"PROG-{f.Random.AlphaNumeric(8)}")
            .RuleFor(d => d.RecipientId, f => $"REC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(d => d.Amount, f => f.Finance.Amount(100, 5000, 2).ToString("F2"))
            .RuleFor(d => d.Currency, "USD")
            .RuleFor(d => d.Purpose, f => f.PickRandom("Unemployment", "Disability", "Housing", "Education", "Healthcare"))
            .RuleFor(d => d.Status, f => f.PickRandom<PaymentStatus>())
            .RuleFor(d => d.CreatedAt, f => f.Date.Recent(60))
            .RuleFor(d => d.ProcessedAt, (f, d) => d.Status == PaymentStatus.Completed ? f.Date.Between(d.CreatedAt, DateTime.Now) : null);

        _disbursements.AddRange(disbursementFaker.Generate(40));

        // Generate recurring payments
        var recurringFaker = new Faker<RecurringPaymentInfo>()
            .RuleFor(r => r.RecurringPaymentId, f => $"RECUR-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(r => r.FromAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(r => r.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(r => r.Amount, f => f.Finance.Amount(50, 1000, 2).ToString("F2"))
            .RuleFor(r => r.Currency, "USD")
            .RuleFor(r => r.Frequency, f => f.PickRandom("daily", "weekly", "monthly", "yearly"))
            .RuleFor(r => r.StartDate, f => f.Date.Past(6))
            .RuleFor(r => r.EndDate, f => f.Date.Future(12))
            .RuleFor(r => r.NextPaymentDate, f => f.Date.Soon(30))
            .RuleFor(r => r.Status, f => f.PickRandom<RecurringPaymentStatus>());

        _recurringPayments.AddRange(recurringFaker.Generate(25));

        // Generate multisig payments
        var multisigFaker = new Faker<MultisigPaymentInfo>()
            .RuleFor(m => m.PaymentId, f => $"MULTI-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(m => m.Signers, f => Enumerable.Range(0, f.Random.Int(3, 5)).Select(_ => $"ACC-{f.Random.AlphaNumeric(8)}").ToList())
            .RuleFor(m => m.RequiredSignatures, f => f.Random.Int(2, 4))
            .RuleFor(m => m.CurrentSignatures, (f, m) => f.Random.Int(0, m.RequiredSignatures))
            .RuleFor(m => m.SignedBy, (f, m) => m.Signers.Take(m.CurrentSignatures).ToList())
            .RuleFor(m => m.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(m => m.Amount, f => f.Finance.Amount(1000, 50000, 2).ToString("F2"))
            .RuleFor(m => m.Currency, "USD")
            .RuleFor(m => m.Status, (f, m) => m.CurrentSignatures >= m.RequiredSignatures ? MultisigPaymentStatus.Completed : MultisigPaymentStatus.PendingSignatures)
            .RuleFor(m => m.CreatedAt, f => f.Date.Recent(30));

        _multisigPayments.AddRange(multisigFaker.Generate(15));
    }

    public Task<List<PaymentInfo>> ListP2PPaymentsAsync(string accountId, int limit = 50, int offset = 0)
    {
        var payments = _p2pPayments
            .Where(p => p.FromAccount == accountId || p.ToAccount == accountId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToList();
        return Task.FromResult(payments);
    }

    public Task<PaymentInfo?> GetP2PPaymentAsync(string paymentId)
    {
        return Task.FromResult(_p2pPayments.FirstOrDefault(p => p.PaymentId == paymentId));
    }

    public Task<List<MerchantPaymentInfo>> ListMerchantPaymentsAsync(string merchantId, int limit = 50, int offset = 0)
    {
        var payments = _merchantPayments
            .Where(p => p.MerchantId == merchantId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToList();
        return Task.FromResult(payments);
    }

    public Task<MerchantPaymentInfo?> GetMerchantPaymentAsync(string paymentId)
    {
        return Task.FromResult(_merchantPayments.FirstOrDefault(p => p.PaymentId == paymentId));
    }

    public Task<List<DisbursementInfo>> ListDisbursementsAsync(string programId, int limit = 50, int offset = 0)
    {
        var disbursements = _disbursements
            .Where(d => string.IsNullOrEmpty(programId) || d.ProgramId == programId)
            .OrderByDescending(d => d.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToList();
        return Task.FromResult(disbursements);
    }

    public Task<DisbursementInfo?> GetDisbursementAsync(string disbursementId)
    {
        return Task.FromResult(_disbursements.FirstOrDefault(d => d.DisbursementId == disbursementId));
    }

    public Task<List<RecurringPaymentInfo>> ListRecurringPaymentsAsync(string accountId, int limit = 50, int offset = 0)
    {
        var payments = _recurringPayments
            .Where(r => r.FromAccount == accountId || r.ToAccount == accountId)
            .OrderByDescending(r => r.StartDate)
            .Skip(offset)
            .Take(limit)
            .ToList();
        return Task.FromResult(payments);
    }

    public Task<List<MultisigPaymentInfo>> ListMultisigPaymentsAsync(string accountId, int limit = 50, int offset = 0)
    {
        var payments = _multisigPayments
            .Where(m => m.Signers.Contains(accountId))
            .OrderByDescending(m => m.CreatedAt)
            .Skip(offset)
            .Take(limit)
            .ToList();
        return Task.FromResult(payments);
    }

    public Task<MultisigPaymentInfo?> GetMultisigPaymentAsync(string paymentId)
    {
        return Task.FromResult(_multisigPayments.FirstOrDefault(m => m.PaymentId == paymentId));
    }
}
