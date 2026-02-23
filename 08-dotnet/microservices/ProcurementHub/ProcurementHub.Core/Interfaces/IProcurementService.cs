using ProcurementHub.Core.Models;

namespace ProcurementHub.Core.Interfaces;

public interface IProcurementService
{
    Task<PurchaseRequisition> CreateRequisitionAsync(PurchaseRequisition requisition);
    Task<PurchaseRequisition?> GetRequisitionByIdAsync(Guid id);
    Task<IEnumerable<PurchaseRequisition>> GetRequisitionsAsync(RequisitionStatus? status = null, string? department = null);
    Task<PurchaseRequisition> UpdateRequisitionAsync(PurchaseRequisition requisition);
    Task<PurchaseRequisition> ApproveRequisitionAsync(Guid id, Guid approvedBy);
    Task<PurchaseRequisition> RejectRequisitionAsync(Guid id, string reason);

    Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder order);
    Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(Guid id);
    Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync(PurchaseOrderStatus? status = null, string? department = null, Guid? vendorId = null);
    Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder order);
    Task<PurchaseOrder> ApprovePurchaseOrderAsync(Guid id, Guid approvedBy);
    Task<PurchaseOrder> ConvertRequisitionToOrderAsync(Guid requisitionId, Guid vendorId, Guid createdBy);

    Task<Tender> CreateTenderAsync(Tender tender);
    Task<Tender?> GetTenderByIdAsync(Guid id);
    Task<IEnumerable<Tender>> GetTendersAsync(TenderStatus? status = null);
    Task<Tender> PublishTenderAsync(Guid id);
    Task<TenderBid> SubmitBidAsync(TenderBid bid);
    Task<TenderBid> EvaluateBidAsync(Guid bidId, decimal technicalScore, decimal financialScore);
    Task<Tender> AwardTenderAsync(Guid tenderId, Guid vendorId, decimal amount);

    Task<GoodsReceipt> CreateReceiptAsync(GoodsReceipt receipt);
    Task<IEnumerable<GoodsReceipt>> GetReceiptsAsync(Guid? purchaseOrderId = null);

    Task<ProcurementStatistics> GetStatisticsAsync(string? department = null);
}

public class ProcurementStatistics
{
    public int TotalRequisitions { get; set; }
    public int PendingRequisitions { get; set; }
    public int TotalPurchaseOrders { get; set; }
    public int OpenPurchaseOrders { get; set; }
    public decimal TotalPOValue { get; set; }
    public int ActiveTenders { get; set; }
    public int TotalBids { get; set; }
    public decimal AverageSavings { get; set; }
    public Dictionary<string, int> OrdersByStatus { get; set; } = new();
    public Dictionary<string, decimal> SpendByDepartment { get; set; } = new();
}
