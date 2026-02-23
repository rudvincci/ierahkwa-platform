namespace SmartAccounting.Application.DTOs;

public class DateRangeFilter
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class FeesReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalFees { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalDue { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int PendingInvoices { get; set; }
    public IEnumerable<FeesReportItemDto> Items { get; set; } = new List<FeesReportItemDto>();
}

public class FeesReportItemDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string? StudentName { get; set; }
    public string? ClassName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public string? Status { get; set; }
}

public class PurchaseReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal TotalReturns { get; set; }
    public decimal NetPurchases { get; set; }
    public int TotalInvoices { get; set; }
    public IEnumerable<PurchaseReportItemDto> Items { get; set; } = new List<PurchaseReportItemDto>();
}

public class PurchaseReportItemDto
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string? SupplierName { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
}

public class StockReportDto
{
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public decimal TotalStockValue { get; set; }
    public IEnumerable<StockReportItemDto> Items { get; set; } = new List<StockReportItemDto>();
}

public class StockReportItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public string? ProductCode { get; set; }
    public string? CategoryName { get; set; }
    public decimal Quantity { get; set; }
    public decimal MinQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public string StockStatus { get; set; } = string.Empty;
}

public class CashReportDto
{
    public DateTime Date { get; set; }
    public decimal OpeningCash { get; set; }
    public decimal TotalReceived { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal ClosingCash { get; set; }
    public IEnumerable<CashTransactionDto> Transactions { get; set; } = new List<CashTransactionDto>();
}

public class CashTransactionDto
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
}

public class JournalReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public int TotalJournals { get; set; }
    public IEnumerable<JournalDto> Journals { get; set; } = new List<JournalDto>();
}

public class DashboardDto
{
    public decimal TotalFeesCollected { get; set; }
    public decimal TotalFeesDue { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal CashBalance { get; set; }
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public IEnumerable<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new List<MonthlyRevenueDto>();
    public IEnumerable<FeeTypeDistributionDto> FeeTypeDistribution { get; set; } = new List<FeeTypeDistributionDto>();
}

public class MonthlyRevenueDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}

public class FeeTypeDistributionDto
{
    public string FeeType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}
