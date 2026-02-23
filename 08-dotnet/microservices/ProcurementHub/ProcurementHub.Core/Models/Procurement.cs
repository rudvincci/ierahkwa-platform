namespace ProcurementHub.Core.Models;

public class PurchaseRequisition
{
    public Guid Id { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Department { get; set; } = string.Empty;
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public RequisitionStatus Status { get; set; }
    public RequisitionPriority Priority { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime RequiredDate { get; set; }
    public string? Justification { get; set; }
    public string? BudgetCode { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<RequisitionItem> Items { get; set; } = new();
}

public class RequisitionItem
{
    public Guid Id { get; set; }
    public Guid RequisitionId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; } = "EA";
    public decimal EstimatedPrice { get; set; }
    public decimal TotalPrice => Quantity * EstimatedPrice;
    public Guid? PreferredVendorId { get; set; }
    public string? PreferredVendorName { get; set; }
}

public class PurchaseOrder
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid? RequisitionId { get; set; }
    public string? RequisitionNumber { get; set; }
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string? VendorContact { get; set; }
    public string? VendorEmail { get; set; }
    public string Department { get; set; } = string.Empty;
    public PurchaseOrderStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? PaymentTerms { get; set; }
    public string? ShippingAddress { get; set; }
    public DateTime? ExpectedDelivery { get; set; }
    public DateTime? ActualDelivery { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PurchaseOrderItem> Items { get; set; } = new();
}

public class PurchaseOrderItem
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; } = "EA";
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    public int ReceivedQuantity { get; set; }
}

public class Tender
{
    public Guid Id { get; set; }
    public string TenderNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TenderType Type { get; set; }
    public TenderStatus Status { get; set; }
    public string Department { get; set; } = string.Empty;
    public decimal EstimatedBudget { get; set; }
    public DateTime PublishDate { get; set; }
    public DateTime SubmissionDeadline { get; set; }
    public DateTime? OpeningDate { get; set; }
    public string? Requirements { get; set; }
    public string? EvaluationCriteria { get; set; }
    public Guid? AwardedVendorId { get; set; }
    public decimal? AwardedAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TenderBid> Bids { get; set; } = new();
}

public class TenderBid
{
    public Guid Id { get; set; }
    public Guid TenderId { get; set; }
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public string? TechnicalProposal { get; set; }
    public string? FinancialProposal { get; set; }
    public BidStatus Status { get; set; }
    public decimal? TechnicalScore { get; set; }
    public decimal? FinancialScore { get; set; }
    public decimal? TotalScore { get; set; }
    public DateTime SubmittedAt { get; set; }
}

public class GoodsReceipt
{
    public Guid Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public Guid PurchaseOrderId { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public DateTime ReceivedDate { get; set; }
    public Guid ReceivedBy { get; set; }
    public string ReceivedByName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public ReceiptStatus Status { get; set; }
    public List<GoodsReceiptItem> Items { get; set; } = new();
}

public class GoodsReceiptItem
{
    public Guid Id { get; set; }
    public Guid ReceiptId { get; set; }
    public Guid OrderItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int ExpectedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public int AcceptedQuantity { get; set; }
    public int RejectedQuantity { get; set; }
    public string? RejectionReason { get; set; }
}

public enum RequisitionStatus { Draft, Submitted, UnderReview, Approved, Rejected, Converted, Cancelled }
public enum RequisitionPriority { Low, Medium, High, Urgent }
public enum PurchaseOrderStatus { Draft, Pending, Approved, Sent, PartiallyReceived, Received, Completed, Cancelled }
public enum TenderType { Open, Restricted, Negotiated, DirectAward }
public enum TenderStatus { Draft, Published, Open, Closed, UnderEvaluation, Awarded, Cancelled }
public enum BidStatus { Submitted, UnderReview, Shortlisted, Rejected, Awarded }
public enum ReceiptStatus { Partial, Complete, WithDiscrepancy }
