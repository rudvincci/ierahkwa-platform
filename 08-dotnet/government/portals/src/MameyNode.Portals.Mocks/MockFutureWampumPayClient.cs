using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockFutureWampumPayClient : IFutureWampumPayClient
{
    private readonly Faker _faker = new();
    private readonly List<PaymentInfo> _p2pPayments = new();
    private readonly List<MerchantPaymentInfo> _merchantPayments = new();
    private readonly List<DisbursementInfo> _disbursements = new();
    private readonly List<RecurringPaymentInfo> _recurringPayments = new();
    private readonly List<MultisigPaymentInfo> _multisigPayments = new();
    private readonly List<PaymentWalletInfo> _wallets = new();

    public MockFutureWampumPayClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
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

        _p2pPayments.AddRange(p2pFaker.Generate(100));

        var merchantFaker = new Faker<MerchantPaymentInfo>()
            .RuleFor(p => p.PaymentId, f => $"MERCH-PAY-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.MerchantId, f => $"MERCH-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.CustomerAccount, f => $"CUST-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.Amount, f => f.Finance.Amount(10, 1000, 2).ToString("F2"))
            .RuleFor(p => p.Currency, "USD")
            .RuleFor(p => p.OrderId, f => $"ORDER-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.Status, f => f.PickRandom<PaymentStatus>())
            .RuleFor(p => p.CreatedAt, f => f.Date.Recent(30));

        _merchantPayments.AddRange(merchantFaker.Generate(150));

        var disbursementFaker = new Faker<DisbursementInfo>()
            .RuleFor(d => d.DisbursementId, f => $"DISB-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(d => d.ProgramId, f => $"PROG-{f.Random.AlphaNumeric(8)}")
            .RuleFor(d => d.RecipientId, f => $"RECIP-{f.Random.AlphaNumeric(8)}")
            .RuleFor(d => d.Amount, f => f.Finance.Amount(500, 5000, 2).ToString("F2"))
            .RuleFor(d => d.Currency, "USD")
            .RuleFor(d => d.Status, f => f.PickRandom<PaymentStatus>())
            .RuleFor(d => d.CreatedAt, f => f.Date.Recent(30));

        _disbursements.AddRange(disbursementFaker.Generate(75));

        var recurringFaker = new Faker<RecurringPaymentInfo>()
            .RuleFor(r => r.RecurringPaymentId, f => $"RECUR-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(r => r.FromAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(r => r.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(r => r.Amount, f => f.Finance.Amount(50, 500, 2).ToString("F2"))
            .RuleFor(r => r.Currency, "USD")
            .RuleFor(r => r.Frequency, f => f.PickRandom("daily", "weekly", "monthly", "yearly"))
            .RuleFor(r => r.StartDate, f => f.Date.Past(6))
            .RuleFor(r => r.EndDate, f => f.Date.Future(12))
            .RuleFor(r => r.NextPaymentDate, f => f.Date.Soon(30))
            .RuleFor(r => r.Status, f => f.PickRandom<RecurringPaymentStatus>());

        _recurringPayments.AddRange(recurringFaker.Generate(50));

        var multisigFaker = new Faker<MultisigPaymentInfo>()
            .RuleFor(m => m.PaymentId, f => $"MULTISIG-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(m => m.Signers, f => Enumerable.Range(0, f.Random.Int(3, 5)).Select(_ => $"ACC-{f.Random.AlphaNumeric(8)}").ToList())
            .RuleFor(m => m.RequiredSignatures, f => f.Random.Int(2, 4))
            .RuleFor(m => m.CurrentSignatures, (f, m) => f.Random.Int(0, m.RequiredSignatures))
            .RuleFor(m => m.SignedBy, (f, m) => m.Signers.Take(m.CurrentSignatures).ToList())
            .RuleFor(m => m.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(m => m.Amount, f => f.Finance.Amount(100, 10000, 2).ToString("F2"))
            .RuleFor(m => m.Currency, "USD")
            .RuleFor(m => m.Status, (f, m) => m.CurrentSignatures >= m.RequiredSignatures ? MultisigPaymentStatus.Completed : MultisigPaymentStatus.PendingSignatures)
            .RuleFor(m => m.CreatedAt, f => f.Date.Recent(30));

        _multisigPayments.AddRange(multisigFaker.Generate(40));

        var walletFaker = new Faker<PaymentWalletInfo>()
            .RuleFor(w => w.InvoiceId, f => $"WALLET-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(w => w.FromAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(w => w.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(w => w.Amount, f => f.Finance.Amount(1000, 50000, 2).ToString("F2"))
            .RuleFor(w => w.Currency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(w => w.Status, f => f.PickRandom("Active", "Inactive", "Pending"))
            .RuleFor(w => w.CreatedAt, f => f.Date.Past(1))
            .RuleFor(w => w.DueDate, f => f.Date.Future(30))
            .RuleFor(w => w.PaidAt, (f, w) => f.Random.Bool(0.7f) ? f.Date.Between(w.CreatedAt, DateTime.Now) : null)
            .RuleFor(w => w.Description, f => f.Lorem.Sentence());

        _wallets.AddRange(walletFaker.Generate(80));
    }

    public Task<List<PaymentInfo>> GetP2PPaymentsAsync(int limit = 50) => Task.FromResult(_p2pPayments.Take(limit).ToList());
    public Task<PaymentInfo?> GetP2PPaymentAsync(string paymentId) => Task.FromResult(_p2pPayments.FirstOrDefault(p => p.PaymentId == paymentId));
    public Task<List<MerchantPaymentInfo>> GetMerchantPaymentsAsync(int limit = 50) => Task.FromResult(_merchantPayments.Take(limit).ToList());
    public Task<MerchantPaymentInfo?> GetMerchantPaymentAsync(string paymentId) => Task.FromResult(_merchantPayments.FirstOrDefault(p => p.PaymentId == paymentId));
    public Task<List<DisbursementInfo>> GetDisbursementsAsync(int limit = 50) => Task.FromResult(_disbursements.Take(limit).ToList());
    public Task<DisbursementInfo?> GetDisbursementAsync(string disbursementId) => Task.FromResult(_disbursements.FirstOrDefault(d => d.DisbursementId == disbursementId));
    public Task<List<RecurringPaymentInfo>> GetRecurringPaymentsAsync(int limit = 50) => Task.FromResult(_recurringPayments.Take(limit).ToList());
    public Task<List<MultisigPaymentInfo>> GetMultisigPaymentsAsync(int limit = 50) => Task.FromResult(_multisigPayments.Take(limit).ToList());
    public Task<MultisigPaymentInfo?> GetMultisigPaymentAsync(string paymentId) => Task.FromResult(_multisigPayments.FirstOrDefault(p => p.PaymentId == paymentId));
    public Task<List<PaymentWalletInfo>> GetWalletsAsync() => Task.FromResult(_wallets);
    public Task<PaymentWalletInfo?> GetWalletAsync(string walletId) => Task.FromResult(_wallets.FirstOrDefault(w => w.InvoiceId == walletId));
}

