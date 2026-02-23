using ContractManager.Core.Interfaces;
using ContractManager.Core.Models;

namespace ContractManager.Infrastructure.Services;

public class ContractService : IContractService
{
    private readonly List<Contract> _contracts = new();
    private readonly List<ContractMilestone> _milestones = new();
    private readonly List<ContractPayment> _payments = new();
    private readonly List<ContractDocument> _documents = new();
    private readonly List<ContractAmendment> _amendments = new();
    private readonly List<Vendor> _vendors = new();

    public Task<Contract> CreateContractAsync(Contract contract)
    {
        contract.Id = Guid.NewGuid();
        contract.ContractNumber = $"CTR-{DateTime.UtcNow:yyyyMM}-{_contracts.Count + 1:D4}";
        contract.Status = ContractStatus.Draft;
        contract.CreatedAt = DateTime.UtcNow;
        _contracts.Add(contract);
        return Task.FromResult(contract);
    }

    public Task<Contract?> GetContractByIdAsync(Guid id) => Task.FromResult(_contracts.FirstOrDefault(c => c.Id == id));
    
    public Task<IEnumerable<Contract>> GetContractsAsync(ContractStatus? status = null, string? department = null, Guid? vendorId = null)
    {
        var q = _contracts.AsEnumerable();
        if (status.HasValue) q = q.Where(c => c.Status == status.Value);
        if (!string.IsNullOrEmpty(department)) q = q.Where(c => c.Department == department);
        if (vendorId.HasValue) q = q.Where(c => c.VendorId == vendorId.Value);
        return Task.FromResult(q);
    }

    public Task<IEnumerable<Contract>> GetExpiringContractsAsync(int daysAhead = 30) =>
        Task.FromResult(_contracts.Where(c => c.Status == ContractStatus.Active && c.EndDate <= DateTime.UtcNow.AddDays(daysAhead)));

    public Task<Contract> UpdateContractAsync(Contract contract) { var e = _contracts.FirstOrDefault(c => c.Id == contract.Id); if (e != null) { e.Title = contract.Title; e.TotalValue = contract.TotalValue; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? contract); }

    public Task<Contract> ApproveContractAsync(Guid id, Guid approvedBy) { var c = _contracts.FirstOrDefault(c => c.Id == id); if (c != null) { c.Status = ContractStatus.Approved; c.ApprovedBy = approvedBy; c.ApprovedAt = DateTime.UtcNow; } return Task.FromResult(c!); }
    public Task<Contract> TerminateContractAsync(Guid id, string reason) { var c = _contracts.FirstOrDefault(c => c.Id == id); if (c != null) { c.Status = ContractStatus.Terminated; c.TerminationDate = DateTime.UtcNow; c.TerminationReason = reason; } return Task.FromResult(c!); }
    public Task<Contract> RenewContractAsync(Guid id, DateTime newEndDate) { var c = _contracts.FirstOrDefault(c => c.Id == id); if (c != null) { c.EndDate = newEndDate; c.Status = ContractStatus.Renewed; c.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(c!); }
    public Task DeleteContractAsync(Guid id) { _contracts.RemoveAll(c => c.Id == id); return Task.CompletedTask; }

    public Task<ContractMilestone> AddMilestoneAsync(ContractMilestone milestone) { milestone.Id = Guid.NewGuid(); milestone.Status = MilestoneStatus.Pending; _milestones.Add(milestone); return Task.FromResult(milestone); }
    public Task<ContractMilestone> CompleteMilestoneAsync(Guid milestoneId) { var m = _milestones.FirstOrDefault(m => m.Id == milestoneId); if (m != null) { m.Status = MilestoneStatus.Completed; m.CompletedDate = DateTime.UtcNow; } return Task.FromResult(m!); }
    public Task<IEnumerable<ContractMilestone>> GetMilestonesAsync(Guid contractId) => Task.FromResult(_milestones.Where(m => m.ContractId == contractId));

    public Task<ContractPayment> AddPaymentAsync(ContractPayment payment) { payment.Id = Guid.NewGuid(); payment.PaymentNumber = $"PAY-{_payments.Count + 1:D5}"; payment.Status = PaymentStatus.Scheduled; _payments.Add(payment); return Task.FromResult(payment); }
    public Task<ContractPayment> RecordPaymentAsync(Guid paymentId, DateTime paidDate, string? invoiceNumber) { var p = _payments.FirstOrDefault(p => p.Id == paymentId); if (p != null) { p.Status = PaymentStatus.Paid; p.PaidDate = paidDate; p.InvoiceNumber = invoiceNumber; } return Task.FromResult(p!); }
    public Task<IEnumerable<ContractPayment>> GetPaymentsAsync(Guid contractId) => Task.FromResult(_payments.Where(p => p.ContractId == contractId));
    public Task<IEnumerable<ContractPayment>> GetOverduePaymentsAsync() => Task.FromResult(_payments.Where(p => p.Status != PaymentStatus.Paid && p.DueDate < DateTime.UtcNow));

    public Task<ContractAmendment> AddAmendmentAsync(ContractAmendment amendment) { amendment.Id = Guid.NewGuid(); amendment.AmendmentNumber = $"AMD-{_amendments.Count + 1:D4}"; amendment.CreatedAt = DateTime.UtcNow; _amendments.Add(amendment); return Task.FromResult(amendment); }
    public Task<IEnumerable<ContractAmendment>> GetAmendmentsAsync(Guid contractId) => Task.FromResult(_amendments.Where(a => a.ContractId == contractId));

    public Task<ContractDocument> AddDocumentAsync(ContractDocument document) { document.Id = Guid.NewGuid(); document.UploadedAt = DateTime.UtcNow; _documents.Add(document); return Task.FromResult(document); }
    public Task<IEnumerable<ContractDocument>> GetDocumentsAsync(Guid contractId) => Task.FromResult(_documents.Where(d => d.ContractId == contractId));

    public Task<Vendor> CreateVendorAsync(Vendor vendor) { vendor.Id = Guid.NewGuid(); vendor.VendorCode = $"VND-{_vendors.Count + 1:D5}"; vendor.Status = VendorStatus.Active; vendor.CreatedAt = DateTime.UtcNow; _vendors.Add(vendor); return Task.FromResult(vendor); }
    public Task<Vendor?> GetVendorByIdAsync(Guid id) => Task.FromResult(_vendors.FirstOrDefault(v => v.Id == id));
    public Task<IEnumerable<Vendor>> GetVendorsAsync(VendorStatus? status = null) => Task.FromResult(status.HasValue ? _vendors.Where(v => v.Status == status.Value) : _vendors.AsEnumerable());
    public Task<Vendor> UpdateVendorAsync(Vendor vendor) { var e = _vendors.FirstOrDefault(v => v.Id == vendor.Id); if (e != null) { e.Name = vendor.Name; e.Email = vendor.Email; } return Task.FromResult(e ?? vendor); }
    public Task<IEnumerable<Contract>> GetVendorContractsAsync(Guid vendorId) => Task.FromResult(_contracts.Where(c => c.VendorId == vendorId));

    public Task<ContractStatistics> GetStatisticsAsync(string? department = null)
    {
        var contracts = string.IsNullOrEmpty(department) ? _contracts : _contracts.Where(c => c.Department == department).ToList();
        return Task.FromResult(new ContractStatistics
        {
            TotalContracts = contracts.Count, ActiveContracts = contracts.Count(c => c.Status == ContractStatus.Active),
            ExpiringThisMonth = contracts.Count(c => c.EndDate.Month == DateTime.UtcNow.Month && c.EndDate.Year == DateTime.UtcNow.Year),
            TotalContractValue = contracts.Sum(c => c.TotalValue),
            PaidThisYear = _payments.Where(p => p.PaidDate?.Year == DateTime.UtcNow.Year).Sum(p => p.Amount),
            PendingPayments = _payments.Where(p => p.Status != PaymentStatus.Paid).Sum(p => p.Amount),
            TotalVendors = _vendors.Count,
            ContractsByType = contracts.GroupBy(c => c.Type.ToString()).ToDictionary(g => g.Key, g => g.Count()),
            ContractsByStatus = contracts.GroupBy(c => c.Status.ToString()).ToDictionary(g => g.Key, g => g.Count())
        });
    }
}
