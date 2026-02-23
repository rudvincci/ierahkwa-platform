namespace ContractManager.Core.Models;

public class Contract
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ContractType Type { get; set; }
    public ContractStatus Status { get; set; }
    public string? Department { get; set; }
    public Guid? VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string? VendorContact { get; set; }
    public string? VendorEmail { get; set; }
    public decimal TotalValue { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? SignedDate { get; set; }
    public bool AutoRenew { get; set; }
    public int? RenewalTermMonths { get; set; }
    public int? NoticeDays { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string? TerminationReason { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? DocumentUrl { get; set; }
    public string? Tags { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<ContractMilestone> Milestones { get; set; } = new();
    public List<ContractPayment> Payments { get; set; } = new();
    public List<ContractDocument> Documents { get; set; } = new();
    public List<ContractAmendment> Amendments { get; set; } = new();
}

public class ContractMilestone
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public decimal? Value { get; set; }
    public MilestoneStatus Status { get; set; }
}

public class ContractPayment
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public PaymentStatus Status { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? Notes { get; set; }
}

public class ContractDocument
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class ContractAmendment
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public string AmendmentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal? ValueChange { get; set; }
    public DateTime? NewEndDate { get; set; }
    public string? DocumentUrl { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Vendor
{
    public Guid Id { get; set; }
    public string VendorCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? TaxId { get; set; }
    public VendorType Type { get; set; }
    public string? Industry { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Website { get; set; }
    public VendorStatus Status { get; set; }
    public int? Rating { get; set; }
    public decimal TotalContractValue { get; set; }
    public int ActiveContracts { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum ContractType { Service, Purchase, Lease, License, Employment, Partnership, NDA, SLA, Maintenance, Consulting, Other }
public enum ContractStatus { Draft, PendingApproval, Approved, Active, Expired, Terminated, Renewed, OnHold }
public enum MilestoneStatus { Pending, InProgress, Completed, Overdue, Cancelled }
public enum PaymentStatus { Scheduled, Invoiced, Paid, Overdue, Cancelled }
public enum VendorType { Supplier, Contractor, Consultant, Agency, Partner, Other }
public enum VendorStatus { Active, Inactive, Blacklisted, PendingApproval }
