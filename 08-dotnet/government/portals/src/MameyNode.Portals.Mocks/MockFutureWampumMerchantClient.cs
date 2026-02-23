using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockFutureWampumMerchantClient : IFutureWampumMerchantClient
{
    private readonly Faker _faker = new();
    private readonly List<MerchantOnboardingInfo> _onboardings = new();
    private readonly List<MerchantSettlementPaymentInfo> _payments = new();
    private readonly List<SettlementInfo> _settlements = new();
    private readonly List<InvoiceInfo> _invoices = new();
    private readonly List<QRCodeInfo> _qrcodes = new();
    private readonly List<MerchantComplianceInfo> _complianceChecks = new();

    public MockFutureWampumMerchantClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var onboardingFaker = new Faker<MerchantOnboardingInfo>()
            .RuleFor(m => m.MerchantId, f => $"MERCH-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(m => m.BusinessName, f => f.Company.CompanyName())
            .RuleFor(m => m.BusinessType, f => f.PickRandom("Retail", "Restaurant", "Service", "Online", "Marketplace"))
            .RuleFor(m => m.Status, f => f.PickRandom("Pending", "UnderReview", "Approved", "Rejected"))
            .RuleFor(m => m.SubmittedAt, f => f.Date.Recent(90))
            .RuleFor(m => m.ApprovedAt, (f, m) => m.Status == "Approved" ? f.Date.Between(m.SubmittedAt, DateTime.Now) : (DateTime?)null)
            .RuleFor(m => m.ContactEmail, f => f.Internet.Email())
            .RuleFor(m => m.ContactPhone, f => f.Phone.PhoneNumber())
            .RuleFor(m => m.Address, f => f.Address.FullAddress());

        _onboardings.AddRange(onboardingFaker.Generate(100));

        var paymentFaker = new Faker<MerchantSettlementPaymentInfo>()
            .RuleFor(p => p.PaymentId, f => $"PAY-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.MerchantId, f => _onboardings[f.Random.Int(0, _onboardings.Count - 1)].MerchantId)
            .RuleFor(p => p.CustomerAccount, f => $"CUST-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.Amount, f => f.Finance.Amount(10, 1000, 2).ToString("F2"))
            .RuleFor(p => p.Currency, "USD")
            .RuleFor(p => p.OrderId, f => $"ORDER-{f.Random.AlphaNumeric(8)}")
            .RuleFor(p => p.Status, f => f.PickRandom<PaymentStatus>())
            .RuleFor(p => p.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(p => p.TerminalId, f => $"TERM-{f.Random.AlphaNumeric(8)}");

        _payments.AddRange(paymentFaker.Generate(500));

        var settlementFaker = new Faker<SettlementInfo>()
            .RuleFor(s => s.SettlementId, f => $"SETTLE-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(s => s.MerchantId, f => _onboardings[f.Random.Int(0, _onboardings.Count - 1)].MerchantId)
            .RuleFor(s => s.SettlementPeriod, f => f.Date.Recent(30).ToString("yyyy-MM"))
            .RuleFor(s => s.TotalAmount, f => f.Finance.Amount(10000, 500000, 2).ToString("F2"))
            .RuleFor(s => s.Currency, "USD")
            .RuleFor(s => s.TransactionCount, f => f.Random.Int(10, 500))
            .RuleFor(s => s.Status, f => f.PickRandom<SettlementStatus>())
            .RuleFor(s => s.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.ProcessedAt, (f, s) => s.Status == SettlementStatus.Completed ? f.Date.Between(s.CreatedAt, DateTime.Now) : null)
            .RuleFor(s => s.FeeAmount, (f, s) => (decimal.Parse(s.TotalAmount) * 0.02m).ToString("F2"))
            .RuleFor(s => s.NetAmount, (f, s) => (decimal.Parse(s.TotalAmount) - decimal.Parse(s.FeeAmount)).ToString("F2"));

        _settlements.AddRange(settlementFaker.Generate(50));

        var invoiceFaker = new Faker<InvoiceInfo>()
            .RuleFor(i => i.InvoiceId, f => $"INV-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(i => i.MerchantId, f => _onboardings[f.Random.Int(0, _onboardings.Count - 1)].MerchantId)
            .RuleFor(i => i.CustomerId, f => $"CUST-{f.Random.AlphaNumeric(8)}")
            .RuleFor(i => i.Amount, f => f.Finance.Amount(50, 5000, 2).ToString("F2"))
            .RuleFor(i => i.Currency, "USD")
            .RuleFor(i => i.Status, f => f.PickRandom("Pending", "Paid", "Overdue", "Cancelled"))
            .RuleFor(i => i.CreatedAt, f => f.Date.Recent(60))
            .RuleFor(i => i.DueDate, (f, i) => f.Date.Between(i.CreatedAt, i.CreatedAt.AddDays(30)))
            .RuleFor(i => i.PaidAt, (f, i) => i.Status == "Paid" && i.DueDate.HasValue ? f.Date.Between(i.CreatedAt, i.DueDate.Value) : null)
            .RuleFor(i => i.Description, f => f.Lorem.Sentence())
            .RuleFor(i => i.LineItems, f => f.Lorem.Words(f.Random.Int(1, 5)).ToList());

        _invoices.AddRange(invoiceFaker.Generate(300));

        var qrFaker = new Faker<QRCodeInfo>()
            .RuleFor(q => q.QRCodeId, f => $"QR-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(q => q.MerchantId, f => _onboardings[f.Random.Int(0, _onboardings.Count - 1)].MerchantId)
            .RuleFor(q => q.Amount, f => f.Finance.Amount(10, 500, 2).ToString("F2"))
            .RuleFor(q => q.Currency, "USD")
            .RuleFor(q => q.QRCodeData, f => $"https://pay.futurewampum.com/qr/{f.Random.AlphaNumeric(16)}")
            .RuleFor(q => q.QRCodeImageUrl, f => $"https://api.futurewampum.com/qr/{f.Random.AlphaNumeric(16)}.png")
            .RuleFor(q => q.CreatedAt, f => f.Date.Recent(7))
            .RuleFor(q => q.ExpiresAt, (f, q) => f.Date.Between(q.CreatedAt, q.CreatedAt.AddDays(1)))
            .RuleFor(q => q.IsUsed, f => f.Random.Bool(0.3f))
            .RuleFor(q => q.Status, f => f.PickRandom("Active", "Used", "Expired"));

        _qrcodes.AddRange(qrFaker.Generate(200));

        var complianceFaker = new Faker<MerchantComplianceInfo>()
            .RuleFor(c => c.ComplianceId, f => $"COMP-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.MerchantId, f => _onboardings[f.Random.Int(0, _onboardings.Count - 1)].MerchantId)
            .RuleFor(c => c.ComplianceType, f => f.PickRandom("KYC", "AML", "PCI", "GDPR"))
            .RuleFor(c => c.Status, f => f.PickRandom("Pending", "Verified", "Rejected", "Expired"))
            .RuleFor(c => c.CheckedAt, f => f.Date.Recent(180))
            .RuleFor(c => c.IsCompliant, f => f.Random.Bool(0.9f))
            .RuleFor(c => c.Violations, f => f.Random.Bool(0.1f) ? f.Lorem.Words(f.Random.Int(1, 2)).ToList() : new List<string>())
            .RuleFor(c => c.ExpiresAt, (f, c) => f.Date.Between(c.CheckedAt, c.CheckedAt.AddYears(1)));

        _complianceChecks.AddRange(complianceFaker.Generate(150));
    }

    public Task<List<MerchantOnboardingInfo>> GetMerchantOnboardingsAsync() => Task.FromResult(_onboardings);
    public Task<List<MerchantSettlementPaymentInfo>> GetMerchantPaymentsAsync() => Task.FromResult(_payments);
    public Task<List<SettlementInfo>> GetSettlementsAsync() => Task.FromResult(_settlements);
    public Task<List<InvoiceInfo>> GetInvoicesAsync() => Task.FromResult(_invoices);
    public Task<List<QRCodeInfo>> GetQRCodesAsync() => Task.FromResult(_qrcodes);
    public Task<MerchantAnalyticsInfo?> GetMerchantAnalyticsAsync(string merchantId, string period)
    {
        var merchantPayments = _payments.Where(p => p.MerchantId == merchantId).ToList();
        return Task.FromResult<MerchantAnalyticsInfo?>(new MerchantAnalyticsInfo
        {
            MerchantId = merchantId,
            Period = period,
            TotalRevenue = merchantPayments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => decimal.Parse(p.Amount)).ToString("F2"),
            TotalTransactions = merchantPayments.Count.ToString(),
            AverageTransaction = merchantPayments.Any() ? (merchantPayments.Sum(p => decimal.Parse(p.Amount)) / merchantPayments.Count).ToString("F2") : "0",
            TransactionCount = merchantPayments.Count,
            TransactionsByStatus = merchantPayments.GroupBy(p => p.Status).ToDictionary(g => g.Key.ToString(), g => g.Count()),
            RevenueByCurrency = merchantPayments.GroupBy(p => p.Currency).ToDictionary(g => g.Key, g => g.Sum(p => decimal.Parse(p.Amount)).ToString("F2")),
            PeriodStart = DateTime.Now.AddDays(-30),
            PeriodEnd = DateTime.Now
        });
    }
    public Task<List<MerchantComplianceInfo>> GetMerchantComplianceChecksAsync() => Task.FromResult(_complianceChecks);
}

