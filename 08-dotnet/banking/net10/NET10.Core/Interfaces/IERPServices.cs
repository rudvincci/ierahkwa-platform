using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NET10.Core.Models.ERP;

namespace NET10.Core.Interfaces
{
    // ═══════════════════════════════════════════════════════════════
    // COMPANY SERVICE
    // ═══════════════════════════════════════════════════════════════
    public interface ICompanyService
    {
        Task<List<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(Guid id);
        Task<Company> CreateAsync(Company company);
        Task<Company> UpdateAsync(Company company);
        Task<bool> DeleteAsync(Guid id);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // CUSTOMER SERVICE
    // ═══════════════════════════════════════════════════════════════
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync(Guid companyId);
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(Guid id);
        Task<List<Customer>> SearchAsync(Guid companyId, string searchTerm);
        Task<CustomerStatement> GetStatementAsync(Guid customerId, DateTime fromDate, DateTime toDate);
    }
    
    public class CustomerStatement
    {
        public Customer Customer { get; set; } = new();
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public List<StatementLine> Lines { get; set; } = new();
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal ClosingBalance { get; set; }
    }
    
    public class StatementLine
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PRODUCT SERVICE
    // ═══════════════════════════════════════════════════════════════
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync(Guid companyId);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> GetBySKUAsync(Guid companyId, string sku);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid id);
        Task<List<Product>> SearchAsync(Guid companyId, string searchTerm);
        Task<List<Product>> GetLowStockAsync(Guid companyId);
        Task<List<ProductCategory>> GetCategoriesAsync(Guid companyId);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // INVOICE SERVICE
    // ═══════════════════════════════════════════════════════════════
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetAllAsync(Guid companyId);
        Task<Invoice?> GetByIdAsync(Guid id);
        Task<Invoice?> GetByNumberAsync(Guid companyId, string invoiceNumber);
        Task<Invoice> CreateAsync(Invoice invoice);
        Task<Invoice> UpdateAsync(Invoice invoice);
        Task<bool> DeleteAsync(Guid id);
        Task<string> GenerateInvoiceNumberAsync(Guid companyId);
        Task<Invoice> SendInvoiceAsync(Guid invoiceId, string email);
        Task<List<Invoice>> GetByCustomerAsync(Guid customerId);
        Task<List<Invoice>> GetOverdueAsync(Guid companyId);
        Task<InvoiceSummary> GetSummaryAsync(Guid companyId, DateTime fromDate, DateTime toDate);
    }
    
    public class InvoiceSummary
    {
        public int TotalInvoices { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int PaidCount { get; set; }
        public int UnpaidCount { get; set; }
        public int OverdueCount { get; set; }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PAYMENT SERVICE
    // ═══════════════════════════════════════════════════════════════
    public interface IPaymentService
    {
        Task<List<Payment>> GetAllAsync(Guid companyId);
        Task<Payment?> GetByIdAsync(Guid id);
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment> ApplyToInvoiceAsync(Guid paymentId, Guid invoiceId, decimal amount);
        Task<List<Payment>> GetByCustomerAsync(Guid customerId);
        Task<string> GeneratePaymentNumberAsync(Guid companyId);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // INVENTORY SERVICE
    // ═══════════════════════════════════════════════════════════════
    public interface IInventoryService
    {
        Task<List<InventoryStock>> GetStockLevelsAsync(Guid companyId);
        Task<InventoryStock?> GetStockAsync(Guid productId, Guid warehouseId);
        Task<InventoryTransaction> RecordTransactionAsync(InventoryTransaction transaction);
        Task<List<InventoryTransaction>> GetTransactionsAsync(Guid productId, DateTime fromDate, DateTime toDate);
        Task<StockAdjustment> CreateAdjustmentAsync(StockAdjustment adjustment);
        Task<StockAdjustment> ApproveAdjustmentAsync(Guid adjustmentId, string approvedBy);
        Task<List<Warehouse>> GetWarehousesAsync(Guid companyId);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // SUPPLIER SERVICE
    // ═══════════════════════════════════════════════════════════════
    public interface ISupplierService
    {
        Task<List<Supplier>> GetAllAsync(Guid companyId);
        Task<Supplier?> GetByIdAsync(Guid id);
        Task<Supplier> CreateAsync(Supplier supplier);
        Task<Supplier> UpdateAsync(Supplier supplier);
        Task<bool> DeleteAsync(Guid id);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // PURCHASE ORDER SERVICE
    // ═══════════════════════════════════════════════════════════════
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrder>> GetAllAsync(Guid companyId);
        Task<PurchaseOrder?> GetByIdAsync(Guid id);
        Task<PurchaseOrder> CreateAsync(PurchaseOrder po);
        Task<PurchaseOrder> UpdateAsync(PurchaseOrder po);
        Task<PurchaseOrder> ReceiveItemsAsync(Guid poId, List<ReceiveItem> items);
        Task<string> GeneratePONumberAsync(Guid companyId);
    }
    
    public class ReceiveItem
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
    
    // ═══════════════════════════════════════════════════════════════
    // ACCOUNTING SERVICE
    // ═══════════════════════════════════════════════════════════════
    public interface IAccountingService
    {
        // Chart of Accounts
        Task<List<Account>> GetChartOfAccountsAsync(Guid companyId);
        Task<Account?> GetAccountByIdAsync(Guid id);
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> UpdateAccountAsync(Account account);
        
        // Journal Entries
        Task<JournalEntry> CreateJournalEntryAsync(JournalEntry entry);
        Task<JournalEntry> PostJournalEntryAsync(Guid entryId);
        Task<List<JournalEntry>> GetJournalEntriesAsync(Guid companyId, DateTime fromDate, DateTime toDate);
        
        // Tax Rates
        Task<List<TaxRate>> GetTaxRatesAsync(Guid companyId);
        Task<TaxRate> CreateTaxRateAsync(TaxRate taxRate);
        
        // Reports
        Task<TrialBalance> GetTrialBalanceAsync(Guid companyId, DateTime asOfDate);
        Task<ProfitAndLoss> GetProfitAndLossAsync(Guid companyId, DateTime fromDate, DateTime toDate);
        Task<BalanceSheet> GetBalanceSheetAsync(Guid companyId, DateTime asOfDate);
    }
    
    public class TrialBalance
    {
        public DateTime AsOfDate { get; set; }
        public List<TrialBalanceLine> Lines { get; set; } = new();
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
    }
    
    public class TrialBalanceLine
    {
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
    
    public class ProfitAndLoss
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<PLSection> Revenue { get; set; } = new();
        public List<PLSection> Expenses { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit => TotalRevenue - TotalExpenses;
    }
    
    public class PLSection
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public List<PLLine> Lines { get; set; } = new();
    }
    
    public class PLLine
    {
        public string AccountName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
    
    public class BalanceSheet
    {
        public DateTime AsOfDate { get; set; }
        public List<BSSection> Assets { get; set; } = new();
        public List<BSSection> Liabilities { get; set; } = new();
        public List<BSSection> Equity { get; set; } = new();
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
    }
    
    public class BSSection
    {
        public string Category { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<BSLine> Lines { get; set; } = new();
    }
    
    public class BSLine
    {
        public string AccountName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
