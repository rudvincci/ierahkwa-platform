using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;

namespace NET10.API.Controllers;

/// <summary>
/// Report Controller - Financial and operational reports
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    
    public ReportController(
        IInvoiceService invoiceService,
        ICustomerService customerService,
        IProductService productService)
    {
        _invoiceService = invoiceService;
        _customerService = customerService;
        _productService = productService;
    }
    
    /// <summary>
    /// Get sales report
    /// </summary>
    [HttpGet("sales")]
    public async Task<ActionResult<SalesReport>> GetSalesReport(
        Guid companyId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var invoices = await _invoiceService.GetAllAsync(companyId);
        var filtered = invoices.Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate).ToList();
        
        var report = new SalesReport
        {
            CompanyId = companyId,
            FromDate = fromDate,
            ToDate = toDate,
            GeneratedAt = DateTime.UtcNow,
            
            TotalSales = filtered.Sum(i => i.Total),
            TotalTax = filtered.Sum(i => i.TaxAmount),
            TotalInvoices = filtered.Count,
            PaidInvoices = filtered.Count(i => i.AmountPaid >= i.Total),
            UnpaidInvoices = filtered.Count(i => i.AmountPaid < i.Total),
            TotalReceived = filtered.Sum(i => i.AmountPaid),
            TotalOutstanding = filtered.Sum(i => i.Balance),
            
            SalesByMonth = filtered
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlySales
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalSales = g.Sum(i => i.Total),
                    InvoiceCount = g.Count()
                })
                .ToList(),
            
            TopCustomers = filtered
                .GroupBy(i => i.CustomerId)
                .OrderByDescending(g => g.Sum(i => i.Total))
                .Take(10)
                .Select(g => new CustomerSales
                {
                    CustomerId = g.Key,
                    CustomerName = g.First().CustomerName,
                    TotalSales = g.Sum(i => i.Total),
                    InvoiceCount = g.Count()
                })
                .ToList(),
            
            TopProducts = filtered
                .SelectMany(i => i.Items)
                .GroupBy(item => item.ProductId)
                .OrderByDescending(g => g.Sum(item => item.LineTotal))
                .Take(10)
                .Select(g => new ProductSales
                {
                    ProductId = g.Key ?? Guid.Empty,
                    ProductName = g.First().Description,
                    TotalSales = g.Sum(item => item.LineTotal),
                    QuantitySold = g.Sum(item => item.Quantity)
                })
                .ToList()
        };
        
        return Ok(report);
    }
    
    /// <summary>
    /// Get accounts receivable aging report
    /// </summary>
    [HttpGet("aging")]
    public async Task<ActionResult<AgingReport>> GetAgingReport(Guid companyId)
    {
        var invoices = await _invoiceService.GetAllAsync(companyId);
        var unpaid = invoices.Where(i => i.Balance > 0).ToList();
        var today = DateTime.UtcNow.Date;
        
        var report = new AgingReport
        {
            CompanyId = companyId,
            GeneratedAt = DateTime.UtcNow,
            
            Current = unpaid.Where(i => (today - i.DueDate).Days <= 0).Sum(i => i.Balance),
            Days1to30 = unpaid.Where(i => { var d = (today - i.DueDate).Days; return d > 0 && d <= 30; }).Sum(i => i.Balance),
            Days31to60 = unpaid.Where(i => { var d = (today - i.DueDate).Days; return d > 30 && d <= 60; }).Sum(i => i.Balance),
            Days61to90 = unpaid.Where(i => { var d = (today - i.DueDate).Days; return d > 60 && d <= 90; }).Sum(i => i.Balance),
            Over90Days = unpaid.Where(i => (today - i.DueDate).Days > 90).Sum(i => i.Balance),
            
            CustomerAging = unpaid
                .GroupBy(i => i.CustomerId)
                .Select(g => new CustomerAging
                {
                    CustomerId = g.Key,
                    CustomerName = g.First().CustomerName,
                    Current = g.Where(i => (today - i.DueDate).Days <= 0).Sum(i => i.Balance),
                    Days1to30 = g.Where(i => { var d = (today - i.DueDate).Days; return d > 0 && d <= 30; }).Sum(i => i.Balance),
                    Days31to60 = g.Where(i => { var d = (today - i.DueDate).Days; return d > 30 && d <= 60; }).Sum(i => i.Balance),
                    Days61to90 = g.Where(i => { var d = (today - i.DueDate).Days; return d > 60 && d <= 90; }).Sum(i => i.Balance),
                    Over90Days = g.Where(i => (today - i.DueDate).Days > 90).Sum(i => i.Balance)
                })
                .OrderByDescending(c => c.Total)
                .ToList()
        };
        
        report.TotalOutstanding = report.Current + report.Days1to30 + report.Days31to60 + report.Days61to90 + report.Over90Days;
        
        return Ok(report);
    }
    
    /// <summary>
    /// Get customer statement
    /// </summary>
    [HttpGet("customer-statement/{customerId}")]
    public async Task<ActionResult<CustomerStatement>> GetCustomerStatement(
        Guid customerId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var statement = await _customerService.GetStatementAsync(customerId, fromDate, toDate);
        return Ok(statement);
    }
    
    /// <summary>
    /// Get product performance report
    /// </summary>
    [HttpGet("products")]
    public async Task<ActionResult<ProductReport>> GetProductReport(
        Guid companyId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var invoices = await _invoiceService.GetAllAsync(companyId);
        var filtered = invoices.Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate).ToList();
        var products = await _productService.GetAllAsync(companyId);
        
        var report = new ProductReport
        {
            CompanyId = companyId,
            FromDate = fromDate,
            ToDate = toDate,
            GeneratedAt = DateTime.UtcNow,
            
            TotalProducts = products.Count,
            ActiveProducts = products.Count(p => p.Status == NET10.Core.Models.ERP.ProductStatus.Active),
            LowStockProducts = products.Count(p => p.StockQuantity <= p.ReorderLevel),
            
            ProductPerformance = filtered
                .SelectMany(i => i.Items)
                .GroupBy(item => item.ProductId)
                .Select(g =>
                {
                    var product = products.FirstOrDefault(p => p.Id == g.Key);
                    return new ProductPerformance
                    {
                        ProductId = g.Key ?? Guid.Empty,
                        SKU = product?.SKU ?? "",
                        ProductName = product?.Name ?? g.First().Description,
                        QuantitySold = g.Sum(item => item.Quantity),
                        Revenue = g.Sum(item => item.LineTotal),
                        AveragePrice = g.Average(item => item.UnitPrice),
                        TransactionCount = g.Count()
                    };
                })
                .OrderByDescending(p => p.Revenue)
                .ToList()
        };
        
        report.TotalRevenue = report.ProductPerformance.Sum(p => p.Revenue);
        report.TotalQuantitySold = report.ProductPerformance.Sum(p => p.QuantitySold);
        
        return Ok(report);
    }
    
    /// <summary>
    /// Get tax report
    /// </summary>
    [HttpGet("tax")]
    public async Task<ActionResult<TaxReport>> GetTaxReport(
        Guid companyId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        var invoices = await _invoiceService.GetAllAsync(companyId);
        var filtered = invoices.Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate).ToList();
        
        var report = new TaxReport
        {
            CompanyId = companyId,
            FromDate = fromDate,
            ToDate = toDate,
            GeneratedAt = DateTime.UtcNow,
            
            TotalSales = filtered.Sum(i => i.Subtotal),
            TotalTaxCollected = filtered.Sum(i => i.TaxAmount),
            
            TaxByRate = filtered
                .SelectMany(i => i.Items)
                .Where(item => item.IsTaxable)
                .GroupBy(item => item.TaxRate)
                .Select(g => new TaxByRate
                {
                    TaxRate = g.Key,
                    TaxableSales = g.Sum(item => item.LineTotal),
                    TaxAmount = g.Sum(item => item.TaxAmount)
                })
                .OrderByDescending(t => t.TaxRate)
                .ToList(),
            
            TaxByMonth = filtered
                .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlyTax
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TaxableSales = g.Sum(i => i.Subtotal),
                    TaxCollected = g.Sum(i => i.TaxAmount)
                })
                .ToList()
        };
        
        return Ok(report);
    }
    
    /// <summary>
    /// Export report to PDF (placeholder)
    /// </summary>
    [HttpGet("export/{reportType}")]
    public ActionResult ExportReport(
        string reportType,
        Guid companyId,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] string format = "pdf")
    {
        // In a real implementation, this would generate a PDF or Excel file
        return Ok(new
        {
            message = $"Export {reportType} report",
            format,
            companyId,
            fromDate,
            toDate,
            status = "Feature coming soon - PDF generation requires additional libraries"
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// REPORT MODELS
// ═══════════════════════════════════════════════════════════════

public class SalesReport
{
    public Guid CompanyId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    
    public decimal TotalSales { get; set; }
    public decimal TotalTax { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int UnpaidInvoices { get; set; }
    public decimal TotalReceived { get; set; }
    public decimal TotalOutstanding { get; set; }
    
    public List<MonthlySales> SalesByMonth { get; set; } = new();
    public List<CustomerSales> TopCustomers { get; set; } = new();
    public List<ProductSales> TopProducts { get; set; } = new();
}

public class MonthlySales
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalSales { get; set; }
    public int InvoiceCount { get; set; }
}

public class CustomerSales
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public int InvoiceCount { get; set; }
}

public class ProductSales
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal TotalSales { get; set; }
    public decimal QuantitySold { get; set; }
}

public class AgingReport
{
    public Guid CompanyId { get; set; }
    public DateTime GeneratedAt { get; set; }
    
    public decimal Current { get; set; }
    public decimal Days1to30 { get; set; }
    public decimal Days31to60 { get; set; }
    public decimal Days61to90 { get; set; }
    public decimal Over90Days { get; set; }
    public decimal TotalOutstanding { get; set; }
    
    public List<CustomerAging> CustomerAging { get; set; } = new();
}

public class CustomerAging
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Current { get; set; }
    public decimal Days1to30 { get; set; }
    public decimal Days31to60 { get; set; }
    public decimal Days61to90 { get; set; }
    public decimal Over90Days { get; set; }
    public decimal Total => Current + Days1to30 + Days31to60 + Days61to90 + Over90Days;
}

public class ProductReport
{
    public Guid CompanyId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int LowStockProducts { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalQuantitySold { get; set; }
    
    public List<ProductPerformance> ProductPerformance { get; set; } = new();
}

public class ProductPerformance
{
    public Guid ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal QuantitySold { get; set; }
    public decimal Revenue { get; set; }
    public decimal AveragePrice { get; set; }
    public int TransactionCount { get; set; }
}

public class TaxReport
{
    public Guid CompanyId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    
    public decimal TotalSales { get; set; }
    public decimal TotalTaxCollected { get; set; }
    
    public List<TaxByRate> TaxByRate { get; set; } = new();
    public List<MonthlyTax> TaxByMonth { get; set; } = new();
}

public class TaxByRate
{
    public decimal TaxRate { get; set; }
    public decimal TaxableSales { get; set; }
    public decimal TaxAmount { get; set; }
}

public class MonthlyTax
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TaxableSales { get; set; }
    public decimal TaxCollected { get; set; }
}
