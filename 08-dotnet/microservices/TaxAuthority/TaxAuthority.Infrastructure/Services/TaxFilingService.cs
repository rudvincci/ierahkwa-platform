using TaxAuthority.Core.Interfaces;
using TaxAuthority.Core.Models;

namespace TaxAuthority.Infrastructure.Services;

public class TaxFilingService : ITaxFilingService
{
    private static readonly List<TaxFiling> _filings = new()
    {
        new TaxFiling
        {
            Id = "TAX-2025-001",
            CitizenId = "CIT-001",
            CitizenName = "Ruddie Salomon",
            TaxYear = "2025",
            FilingType = "Annual",
            Period = "Full Year",
            GrossIncome = 125000,
            TotalDeductions = 15000,
            TaxableIncome = 110000,
            TaxOwed = 18750,
            TaxPaid = 18750,
            Status = FilingStatus.Approved,
            ComplianceScore = 98,
            SubmittedAt = DateTime.UtcNow.AddDays(-5),
            ApprovedAt = DateTime.UtcNow.AddDays(-2)
        },
        new TaxFiling
        {
            Id = "TAX-2026-Q1",
            CitizenId = "CIT-001",
            CitizenName = "Ruddie Salomon",
            TaxYear = "2026",
            FilingType = "Quarterly",
            Period = "Q1",
            GrossIncome = 32000,
            TotalDeductions = 4000,
            TaxableIncome = 28000,
            TaxOwed = 4800,
            Status = FilingStatus.Pending,
            DueDate = new DateTime(2026, 4, 15)
        }
    };

    public Task<IEnumerable<TaxFiling>> GetAllAsync() => Task.FromResult(_filings.AsEnumerable());

    public Task<TaxFiling?> GetByIdAsync(string id) => Task.FromResult(_filings.FirstOrDefault(f => f.Id == id));

    public Task<IEnumerable<TaxFiling>> GetByCitizenIdAsync(string citizenId) => 
        Task.FromResult(_filings.Where(f => f.CitizenId == citizenId));

    public Task<TaxFiling> CreateAsync(TaxFiling filing)
    {
        filing.Id = $"TAX-{DateTime.Now.Year}-{(_filings.Count + 1):D3}";
        filing.TaxableIncome = filing.GrossIncome - filing.TotalDeductions;
        filing.TaxOwed = filing.TaxableIncome * filing.TaxRate;
        filing.BalanceDue = filing.TaxOwed - filing.TaxPaid;
        _filings.Add(filing);
        return Task.FromResult(filing);
    }

    public Task UpdateAsync(TaxFiling filing)
    {
        var index = _filings.FindIndex(f => f.Id == filing.Id);
        if (index >= 0) _filings[index] = filing;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        _filings.RemoveAll(f => f.Id == id);
        return Task.CompletedTask;
    }

    public Task SubmitFilingAsync(string id)
    {
        var filing = _filings.FirstOrDefault(f => f.Id == id);
        if (filing != null)
        {
            filing.Status = FilingStatus.Pending;
            filing.SubmittedAt = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    public Task<TaxStats> GetStatsAsync()
    {
        return Task.FromResult(new TaxStats
        {
            TotalCollected = _filings.Where(f => f.Status == FilingStatus.Approved).Sum(f => f.TaxPaid),
            TotalFilings = _filings.Count,
            PendingFilings = _filings.Count(f => f.Status == FilingStatus.Pending),
            ApprovedFilings = _filings.Count(f => f.Status == FilingStatus.Approved),
            ComplianceRate = 98.0m,
            NextDeadline = new DateTime(2026, 4, 15)
        });
    }

    public Task<decimal> CalculateTax(decimal income, decimal deductions)
    {
        var taxable = income - deductions;
        return Task.FromResult(taxable * 0.15m);
    }
}
