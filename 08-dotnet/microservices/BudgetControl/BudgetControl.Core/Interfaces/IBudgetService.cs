using BudgetControl.Core.Models;
namespace BudgetControl.Core.Interfaces;

public interface IBudgetService
{
    Task<Budget> CreateBudgetAsync(Budget budget);
    Task<Budget?> GetBudgetByIdAsync(Guid id);
    Task<IEnumerable<Budget>> GetBudgetsAsync(int? fiscalYear = null, string? department = null, BudgetStatus? status = null);
    Task<Budget> UpdateBudgetAsync(Budget budget);
    Task<Budget> ApproveBudgetAsync(Guid id, Guid approvedBy);
    Task<Budget> FreezeBudgetAsync(Guid id);

    Task<BudgetLine> AddLineAsync(BudgetLine line);
    Task<IEnumerable<BudgetLine>> GetLinesAsync(Guid budgetId);
    Task<BudgetLine> UpdateLineAsync(BudgetLine line);

    Task<BudgetTransaction> RecordTransactionAsync(BudgetTransaction transaction);
    Task<IEnumerable<BudgetTransaction>> GetTransactionsAsync(Guid budgetId, DateTime? from = null, DateTime? to = null);
    Task<bool> CheckAvailabilityAsync(Guid budgetId, decimal amount);

    Task<BudgetTransfer> RequestTransferAsync(BudgetTransfer transfer);
    Task<BudgetTransfer> ApproveTransferAsync(Guid transferId, Guid approvedBy);
    Task<IEnumerable<BudgetTransfer>> GetTransfersAsync(TransferStatus? status = null);

    Task<BudgetForecast> CreateForecastAsync(BudgetForecast forecast);
    Task<IEnumerable<BudgetForecast>> GetForecastsAsync(Guid budgetId, int year);
    Task UpdateForecastActualsAsync(Guid budgetId, int month, int year);

    Task<BudgetStatistics> GetStatisticsAsync(int? fiscalYear = null, string? department = null);
}

public class BudgetStatistics
{
    public decimal TotalAllocated { get; set; }
    public decimal TotalCommitted { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalAvailable { get; set; }
    public double OverallUtilization { get; set; }
    public int TotalBudgets { get; set; }
    public int ActiveBudgets { get; set; }
    public int OverBudgetCount { get; set; }
    public Dictionary<string, decimal> AllocationByDepartment { get; set; } = new();
    public Dictionary<string, decimal> SpendingByCategory { get; set; } = new();
    public List<MonthlySpending> MonthlyTrend { get; set; } = new();
}

public class MonthlySpending { public int Month { get; set; } public int Year { get; set; } public decimal Amount { get; set; } }
