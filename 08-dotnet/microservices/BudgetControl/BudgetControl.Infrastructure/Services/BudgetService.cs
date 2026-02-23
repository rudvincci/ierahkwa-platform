using BudgetControl.Core.Interfaces;
using BudgetControl.Core.Models;
namespace BudgetControl.Infrastructure.Services;

public class BudgetService : IBudgetService
{
    private readonly List<Budget> _budgets = new();
    private readonly List<BudgetLine> _lines = new();
    private readonly List<BudgetTransaction> _transactions = new();
    private readonly List<BudgetTransfer> _transfers = new();
    private readonly List<BudgetForecast> _forecasts = new();

    public Task<Budget> CreateBudgetAsync(Budget budget) { budget.Id = Guid.NewGuid(); budget.BudgetCode = $"BDG-{budget.FiscalYear}-{_budgets.Count + 1:D4}"; budget.Status = BudgetStatus.Draft; budget.CreatedAt = DateTime.UtcNow; _budgets.Add(budget); return Task.FromResult(budget); }
    public Task<Budget?> GetBudgetByIdAsync(Guid id) => Task.FromResult(_budgets.FirstOrDefault(b => b.Id == id));
    public Task<IEnumerable<Budget>> GetBudgetsAsync(int? fiscalYear = null, string? department = null, BudgetStatus? status = null) { var q = _budgets.AsEnumerable(); if (fiscalYear.HasValue) q = q.Where(b => b.FiscalYear == fiscalYear.Value); if (!string.IsNullOrEmpty(department)) q = q.Where(b => b.Department == department); if (status.HasValue) q = q.Where(b => b.Status == status.Value); return Task.FromResult(q); }
    public Task<Budget> UpdateBudgetAsync(Budget budget) { var e = _budgets.FirstOrDefault(b => b.Id == budget.Id); if (e != null) { e.Name = budget.Name; e.AllocatedAmount = budget.AllocatedAmount; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? budget); }
    public Task<Budget> ApproveBudgetAsync(Guid id, Guid approvedBy) { var b = _budgets.FirstOrDefault(b => b.Id == id); if (b != null) b.Status = BudgetStatus.Approved; return Task.FromResult(b!); }
    public Task<Budget> FreezeBudgetAsync(Guid id) { var b = _budgets.FirstOrDefault(b => b.Id == id); if (b != null) b.Status = BudgetStatus.Frozen; return Task.FromResult(b!); }

    public Task<BudgetLine> AddLineAsync(BudgetLine line) { line.Id = Guid.NewGuid(); line.LineCode = $"LN-{_lines.Count + 1:D4}"; _lines.Add(line); return Task.FromResult(line); }
    public Task<IEnumerable<BudgetLine>> GetLinesAsync(Guid budgetId) => Task.FromResult(_lines.Where(l => l.BudgetId == budgetId));
    public Task<BudgetLine> UpdateLineAsync(BudgetLine line) { var e = _lines.FirstOrDefault(l => l.Id == line.Id); if (e != null) e.AllocatedAmount = line.AllocatedAmount; return Task.FromResult(e ?? line); }

    public Task<BudgetTransaction> RecordTransactionAsync(BudgetTransaction transaction)
    {
        transaction.Id = Guid.NewGuid(); transaction.TransactionNumber = $"TXN-{DateTime.UtcNow:yyyyMMdd}-{_transactions.Count + 1:D5}"; transaction.CreatedAt = DateTime.UtcNow; _transactions.Add(transaction);
        var budget = _budgets.FirstOrDefault(b => b.Id == transaction.BudgetId);
        if (budget != null) { switch (transaction.Type) { case TransactionType.Commitment: budget.CommittedAmount += transaction.Amount; break; case TransactionType.Expense: budget.SpentAmount += transaction.Amount; break; case TransactionType.Release: budget.CommittedAmount -= transaction.Amount; break; } }
        return Task.FromResult(transaction);
    }
    public Task<IEnumerable<BudgetTransaction>> GetTransactionsAsync(Guid budgetId, DateTime? from = null, DateTime? to = null) { var q = _transactions.Where(t => t.BudgetId == budgetId); if (from.HasValue) q = q.Where(t => t.TransactionDate >= from.Value); if (to.HasValue) q = q.Where(t => t.TransactionDate <= to.Value); return Task.FromResult(q); }
    public Task<bool> CheckAvailabilityAsync(Guid budgetId, decimal amount) { var b = _budgets.FirstOrDefault(b => b.Id == budgetId); return Task.FromResult(b != null && b.AvailableAmount >= amount); }

    public Task<BudgetTransfer> RequestTransferAsync(BudgetTransfer transfer) { transfer.Id = Guid.NewGuid(); transfer.TransferNumber = $"TRF-{_transfers.Count + 1:D4}"; transfer.Status = TransferStatus.Pending; transfer.RequestedAt = DateTime.UtcNow; _transfers.Add(transfer); return Task.FromResult(transfer); }
    public async Task<BudgetTransfer> ApproveTransferAsync(Guid transferId, Guid approvedBy)
    {
        var t = _transfers.FirstOrDefault(t => t.Id == transferId); if (t == null) throw new Exception("Transfer not found");
        t.Status = TransferStatus.Approved; t.ApprovedBy = approvedBy; t.ApprovedAt = DateTime.UtcNow;
        var from = _budgets.FirstOrDefault(b => b.Id == t.FromBudgetId); var to = _budgets.FirstOrDefault(b => b.Id == t.ToBudgetId);
        if (from != null) from.AllocatedAmount -= t.Amount; if (to != null) to.AllocatedAmount += t.Amount;
        await RecordTransactionAsync(new BudgetTransaction { BudgetId = t.FromBudgetId, Type = TransactionType.Transfer, Amount = -t.Amount, Description = $"Transfer to {t.ToBudgetName}", TransactionDate = DateTime.UtcNow, CreatedBy = approvedBy });
        await RecordTransactionAsync(new BudgetTransaction { BudgetId = t.ToBudgetId, Type = TransactionType.Transfer, Amount = t.Amount, Description = $"Transfer from {t.FromBudgetName}", TransactionDate = DateTime.UtcNow, CreatedBy = approvedBy });
        t.Status = TransferStatus.Completed; return t;
    }
    public Task<IEnumerable<BudgetTransfer>> GetTransfersAsync(TransferStatus? status = null) => Task.FromResult(status.HasValue ? _transfers.Where(t => t.Status == status.Value) : _transfers.AsEnumerable());

    public Task<BudgetForecast> CreateForecastAsync(BudgetForecast forecast) { forecast.Id = Guid.NewGuid(); forecast.CreatedAt = DateTime.UtcNow; _forecasts.Add(forecast); return Task.FromResult(forecast); }
    public Task<IEnumerable<BudgetForecast>> GetForecastsAsync(Guid budgetId, int year) => Task.FromResult(_forecasts.Where(f => f.BudgetId == budgetId && f.Year == year));
    public Task UpdateForecastActualsAsync(Guid budgetId, int month, int year) { var f = _forecasts.FirstOrDefault(f => f.BudgetId == budgetId && f.Month == month && f.Year == year); if (f != null) f.ActualAmount = _transactions.Where(t => t.BudgetId == budgetId && t.TransactionDate.Month == month && t.TransactionDate.Year == year).Sum(t => t.Amount); return Task.CompletedTask; }

    public Task<BudgetStatistics> GetStatisticsAsync(int? fiscalYear = null, string? department = null)
    {
        var budgets = _budgets.AsEnumerable(); if (fiscalYear.HasValue) budgets = budgets.Where(b => b.FiscalYear == fiscalYear.Value); if (!string.IsNullOrEmpty(department)) budgets = budgets.Where(b => b.Department == department);
        var list = budgets.ToList();
        return Task.FromResult(new BudgetStatistics { TotalAllocated = list.Sum(b => b.AllocatedAmount), TotalCommitted = list.Sum(b => b.CommittedAmount), TotalSpent = list.Sum(b => b.SpentAmount), TotalAvailable = list.Sum(b => b.AvailableAmount), OverallUtilization = list.Any() ? (double)list.Average(b => b.UtilizationPercent) : 0, TotalBudgets = list.Count, ActiveBudgets = list.Count(b => b.Status == BudgetStatus.Active), OverBudgetCount = list.Count(b => b.AvailableAmount < 0), AllocationByDepartment = list.Where(b => b.Department != null).GroupBy(b => b.Department!).ToDictionary(g => g.Key, g => g.Sum(b => b.AllocatedAmount)) });
    }
}
