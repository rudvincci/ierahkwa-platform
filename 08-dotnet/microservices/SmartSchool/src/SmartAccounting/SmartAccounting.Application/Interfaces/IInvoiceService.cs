using Common.Application.DTOs;
using SmartAccounting.Application.DTOs;
using SmartAccounting.Domain.Entities;

namespace SmartAccounting.Application.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceDto?> GetByIdAsync(int id);
    Task<InvoiceDto?> GetByNumberAsync(string invoiceNumber);
    Task<PagedResult<InvoiceDto>> GetAllAsync(QueryParameters parameters, InvoiceType? type = null);
    Task<IEnumerable<InvoiceDto>> GetByStudentAsync(int studentId);
    Task<IEnumerable<InvoiceDto>> GetBySupplierAsync(int supplierId);
    Task<InvoiceDto> CreateFeesInvoiceAsync(CreateInvoiceDto dto);
    Task<InvoiceDto> CreatePurchaseInvoiceAsync(CreateInvoiceDto dto);
    Task<InvoiceDto> CreateReturnInvoiceAsync(CreateReturnInvoiceDto dto);
    Task<InvoiceDto> AddPaymentAsync(CreatePaymentDto dto);
    Task<bool> CancelInvoiceAsync(int id);
    Task<byte[]> GenerateInvoicePdfAsync(int id);
}

public interface IJournalService
{
    Task<JournalDto?> GetByIdAsync(int id);
    Task<JournalDto?> GetByNumberAsync(string journalNumber);
    Task<PagedResult<JournalDto>> GetAllAsync(QueryParameters parameters);
    Task<JournalDto> CreateAsync(CreateJournalDto dto);
    Task<JournalDto> UpdateAsync(int id, CreateJournalDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> PostAsync(int id);
    Task<bool> ApproveAsync(int id);
}

public interface IAccountService
{
    Task<AccountDto?> GetByIdAsync(int id);
    Task<AccountDto?> GetByCodeAsync(string code);
    Task<PagedResult<AccountDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<AccountDto>> GetTreeAsync();
    Task<AccountDto> CreateAsync(CreateAccountDto dto);
    Task<AccountDto> UpdateAsync(int id, CreateAccountDto dto);
    Task<bool> DeleteAsync(int id);
    Task<decimal> GetBalanceAsync(int id);
}

public interface ICostCenterService
{
    Task<CostCenterDto?> GetByIdAsync(int id);
    Task<PagedResult<CostCenterDto>> GetAllAsync(QueryParameters parameters);
    Task<IEnumerable<CostCenterDto>> GetTreeAsync();
    Task<CostCenterDto> CreateAsync(CreateCostCenterDto dto);
    Task<CostCenterDto> UpdateAsync(int id, CreateCostCenterDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IReportService
{
    Task<FeesReportDto> GetFeesReportAsync(DateRangeFilter filter);
    Task<FeesReportDto> GetFeesReturnReportAsync(DateRangeFilter filter);
    Task<PurchaseReportDto> GetPurchaseReportAsync(DateRangeFilter filter);
    Task<PurchaseReportDto> GetPurchaseReturnReportAsync(DateRangeFilter filter);
    Task<StockReportDto> GetStockReportAsync();
    Task<CashReportDto> GetCashReportAsync(DateTime date);
    Task<JournalReportDto> GetJournalReportAsync(DateRangeFilter filter);
    Task<DashboardDto> GetAccountantDashboardAsync();
    Task<DashboardDto> GetSchoolAdminDashboardAsync();
}
