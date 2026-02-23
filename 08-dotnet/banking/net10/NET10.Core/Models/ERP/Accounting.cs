using System;
using System.Collections.Generic;

namespace NET10.Core.Models.ERP
{
    /// <summary>
    /// Chart of Accounts
    /// </summary>
    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AccountType Type { get; set; }
        public AccountCategory Category { get; set; }
        public Guid? ParentAccountId { get; set; }
        public int Level { get; set; } = 1;
        public bool IsSystemAccount { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public decimal OpeningBalance { get; set; } = 0;
        public decimal CurrentBalance { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public enum AccountType
    {
        Asset,
        Liability,
        Equity,
        Revenue,
        Expense
    }
    
    public enum AccountCategory
    {
        // Assets
        Cash,
        Bank,
        AccountsReceivable,
        Inventory,
        FixedAssets,
        OtherAssets,
        
        // Liabilities
        AccountsPayable,
        CreditCard,
        TaxPayable,
        Loans,
        OtherLiabilities,
        
        // Equity
        OwnersEquity,
        RetainedEarnings,
        
        // Revenue
        Sales,
        OtherIncome,
        
        // Expenses
        CostOfGoodsSold,
        OperatingExpenses,
        Payroll,
        TaxExpense,
        OtherExpenses
    }
    
    /// <summary>
    /// Journal Entry
    /// </summary>
    public class JournalEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string EntryNumber { get; set; } = string.Empty;
        public DateTime EntryDate { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public JournalEntryType Type { get; set; }
        
        // Source Document
        public string SourceType { get; set; } = string.Empty; // Invoice, Payment, etc.
        public Guid? SourceId { get; set; }
        
        // Lines
        public List<JournalLine> Lines { get; set; } = new();
        
        // Status
        public JournalStatus Status { get; set; } = JournalStatus.Draft;
        public bool IsPosted { get; set; } = false;
        public DateTime? PostedAt { get; set; }
        
        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Journal Entry Line (Debit/Credit)
    /// </summary>
    public class JournalLine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid JournalEntryId { get; set; }
        public int LineNumber { get; set; }
        public Guid AccountId { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Debit { get; set; } = 0;
        public decimal Credit { get; set; } = 0;
    }
    
    public enum JournalEntryType
    {
        General,
        Sales,
        Purchase,
        Payment,
        Receipt,
        Adjustment,
        Opening,
        Closing
    }
    
    public enum JournalStatus
    {
        Draft,
        Posted,
        Voided
    }
    
    /// <summary>
    /// Tax Configuration
    /// </summary>
    public class TaxRate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CompanyId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public TaxType Type { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public Guid? SalesAccountId { get; set; }
        public Guid? PurchaseAccountId { get; set; }
    }
    
    public enum TaxType
    {
        GST,
        VAT,
        SalesTax,
        ServiceTax,
        Excise,
        WithholdingTax,
        Other
    }
}
