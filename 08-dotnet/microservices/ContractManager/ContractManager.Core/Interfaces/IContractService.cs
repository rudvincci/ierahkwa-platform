using ContractManager.Core.Models;

namespace ContractManager.Core.Interfaces;

public interface IContractService
{
    Task<Contract> CreateContractAsync(Contract contract);
    Task<Contract?> GetContractByIdAsync(Guid id);
    Task<IEnumerable<Contract>> GetContractsAsync(ContractStatus? status = null, string? department = null, Guid? vendorId = null);
    Task<IEnumerable<Contract>> GetExpiringContractsAsync(int daysAhead = 30);
    Task<Contract> UpdateContractAsync(Contract contract);
    Task<Contract> ApproveContractAsync(Guid id, Guid approvedBy);
    Task<Contract> TerminateContractAsync(Guid id, string reason);
    Task<Contract> RenewContractAsync(Guid id, DateTime newEndDate);
    Task DeleteContractAsync(Guid id);

    Task<ContractMilestone> AddMilestoneAsync(ContractMilestone milestone);
    Task<ContractMilestone> CompleteMilestoneAsync(Guid milestoneId);
    Task<IEnumerable<ContractMilestone>> GetMilestonesAsync(Guid contractId);

    Task<ContractPayment> AddPaymentAsync(ContractPayment payment);
    Task<ContractPayment> RecordPaymentAsync(Guid paymentId, DateTime paidDate, string? invoiceNumber);
    Task<IEnumerable<ContractPayment>> GetPaymentsAsync(Guid contractId);
    Task<IEnumerable<ContractPayment>> GetOverduePaymentsAsync();

    Task<ContractAmendment> AddAmendmentAsync(ContractAmendment amendment);
    Task<IEnumerable<ContractAmendment>> GetAmendmentsAsync(Guid contractId);

    Task<ContractDocument> AddDocumentAsync(ContractDocument document);
    Task<IEnumerable<ContractDocument>> GetDocumentsAsync(Guid contractId);

    Task<Vendor> CreateVendorAsync(Vendor vendor);
    Task<Vendor?> GetVendorByIdAsync(Guid id);
    Task<IEnumerable<Vendor>> GetVendorsAsync(VendorStatus? status = null);
    Task<Vendor> UpdateVendorAsync(Vendor vendor);
    Task<IEnumerable<Contract>> GetVendorContractsAsync(Guid vendorId);

    Task<ContractStatistics> GetStatisticsAsync(string? department = null);
}

public class ContractStatistics
{
    public int TotalContracts { get; set; }
    public int ActiveContracts { get; set; }
    public int ExpiringThisMonth { get; set; }
    public decimal TotalContractValue { get; set; }
    public decimal PaidThisYear { get; set; }
    public decimal PendingPayments { get; set; }
    public int TotalVendors { get; set; }
    public Dictionary<string, int> ContractsByType { get; set; } = new();
    public Dictionary<string, int> ContractsByStatus { get; set; } = new();
    public Dictionary<string, decimal> ValueByDepartment { get; set; } = new();
}
