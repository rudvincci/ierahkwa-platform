using ProcurementHub.Core.Interfaces;
using ProcurementHub.Core.Models;

namespace ProcurementHub.Infrastructure.Services;

public class ProcurementService : IProcurementService
{
    private readonly List<PurchaseRequisition> _requisitions = new();
    private readonly List<PurchaseOrder> _orders = new();
    private readonly List<Tender> _tenders = new();
    private readonly List<TenderBid> _bids = new();
    private readonly List<GoodsReceipt> _receipts = new();

    public Task<PurchaseRequisition> CreateRequisitionAsync(PurchaseRequisition requisition) { requisition.Id = Guid.NewGuid(); requisition.RequisitionNumber = $"PR-{DateTime.UtcNow:yyyyMMdd}-{_requisitions.Count + 1:D4}"; requisition.Status = RequisitionStatus.Draft; requisition.CreatedAt = DateTime.UtcNow; _requisitions.Add(requisition); return Task.FromResult(requisition); }
    public Task<PurchaseRequisition?> GetRequisitionByIdAsync(Guid id) => Task.FromResult(_requisitions.FirstOrDefault(r => r.Id == id));
    public Task<IEnumerable<PurchaseRequisition>> GetRequisitionsAsync(RequisitionStatus? status = null, string? department = null) { var q = _requisitions.AsEnumerable(); if (status.HasValue) q = q.Where(r => r.Status == status.Value); if (!string.IsNullOrEmpty(department)) q = q.Where(r => r.Department == department); return Task.FromResult(q); }
    public Task<PurchaseRequisition> UpdateRequisitionAsync(PurchaseRequisition requisition) { var e = _requisitions.FirstOrDefault(r => r.Id == requisition.Id); if (e != null) { e.Title = requisition.Title; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? requisition); }
    public Task<PurchaseRequisition> ApproveRequisitionAsync(Guid id, Guid approvedBy) { var r = _requisitions.FirstOrDefault(r => r.Id == id); if (r != null) { r.Status = RequisitionStatus.Approved; r.ApprovedBy = approvedBy; r.ApprovedAt = DateTime.UtcNow; } return Task.FromResult(r!); }
    public Task<PurchaseRequisition> RejectRequisitionAsync(Guid id, string reason) { var r = _requisitions.FirstOrDefault(r => r.Id == id); if (r != null) r.Status = RequisitionStatus.Rejected; return Task.FromResult(r!); }

    public Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder order) { order.Id = Guid.NewGuid(); order.OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{_orders.Count + 1:D4}"; order.Status = PurchaseOrderStatus.Draft; order.CreatedAt = DateTime.UtcNow; _orders.Add(order); return Task.FromResult(order); }
    public Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(Guid id) => Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));
    public Task<IEnumerable<PurchaseOrder>> GetPurchaseOrdersAsync(PurchaseOrderStatus? status = null, string? department = null, Guid? vendorId = null) { var q = _orders.AsEnumerable(); if (status.HasValue) q = q.Where(o => o.Status == status.Value); if (!string.IsNullOrEmpty(department)) q = q.Where(o => o.Department == department); if (vendorId.HasValue) q = q.Where(o => o.VendorId == vendorId.Value); return Task.FromResult(q); }
    public Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder order) { var e = _orders.FirstOrDefault(o => o.Id == order.Id); if (e != null) e.TotalAmount = order.TotalAmount; return Task.FromResult(e ?? order); }
    public Task<PurchaseOrder> ApprovePurchaseOrderAsync(Guid id, Guid approvedBy) { var o = _orders.FirstOrDefault(o => o.Id == id); if (o != null) { o.Status = PurchaseOrderStatus.Approved; o.ApprovedBy = approvedBy; o.ApprovedAt = DateTime.UtcNow; } return Task.FromResult(o!); }
    public Task<PurchaseOrder> ConvertRequisitionToOrderAsync(Guid requisitionId, Guid vendorId, Guid createdBy) { var r = _requisitions.FirstOrDefault(r => r.Id == requisitionId); if (r == null) throw new Exception("Requisition not found"); var o = new PurchaseOrder { Id = Guid.NewGuid(), OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{_orders.Count + 1:D4}", RequisitionId = requisitionId, VendorId = vendorId, Department = r.Department, TotalAmount = r.TotalAmount, CreatedBy = createdBy, Status = PurchaseOrderStatus.Draft, CreatedAt = DateTime.UtcNow }; _orders.Add(o); r.Status = RequisitionStatus.Converted; return Task.FromResult(o); }

    public Task<Tender> CreateTenderAsync(Tender tender) { tender.Id = Guid.NewGuid(); tender.TenderNumber = $"TND-{DateTime.UtcNow:yyyyMM}-{_tenders.Count + 1:D4}"; tender.Status = TenderStatus.Draft; tender.CreatedAt = DateTime.UtcNow; _tenders.Add(tender); return Task.FromResult(tender); }
    public Task<Tender?> GetTenderByIdAsync(Guid id) => Task.FromResult(_tenders.FirstOrDefault(t => t.Id == id));
    public Task<IEnumerable<Tender>> GetTendersAsync(TenderStatus? status = null) => Task.FromResult(status.HasValue ? _tenders.Where(t => t.Status == status.Value) : _tenders.AsEnumerable());
    public Task<Tender> PublishTenderAsync(Guid id) { var t = _tenders.FirstOrDefault(t => t.Id == id); if (t != null) { t.Status = TenderStatus.Published; t.PublishDate = DateTime.UtcNow; } return Task.FromResult(t!); }
    public Task<TenderBid> SubmitBidAsync(TenderBid bid) { bid.Id = Guid.NewGuid(); bid.Status = BidStatus.Submitted; bid.SubmittedAt = DateTime.UtcNow; _bids.Add(bid); return Task.FromResult(bid); }
    public Task<TenderBid> EvaluateBidAsync(Guid bidId, decimal technicalScore, decimal financialScore) { var b = _bids.FirstOrDefault(b => b.Id == bidId); if (b != null) { b.TechnicalScore = technicalScore; b.FinancialScore = financialScore; b.TotalScore = (technicalScore * 0.7m) + (financialScore * 0.3m); b.Status = BidStatus.UnderReview; } return Task.FromResult(b!); }
    public Task<Tender> AwardTenderAsync(Guid tenderId, Guid vendorId, decimal amount) { var t = _tenders.FirstOrDefault(t => t.Id == tenderId); if (t != null) { t.Status = TenderStatus.Awarded; t.AwardedVendorId = vendorId; t.AwardedAmount = amount; var b = _bids.FirstOrDefault(b => b.TenderId == tenderId && b.VendorId == vendorId); if (b != null) b.Status = BidStatus.Awarded; } return Task.FromResult(t!); }

    public Task<GoodsReceipt> CreateReceiptAsync(GoodsReceipt receipt) { receipt.Id = Guid.NewGuid(); receipt.ReceiptNumber = $"GR-{DateTime.UtcNow:yyyyMMdd}-{_receipts.Count + 1:D4}"; _receipts.Add(receipt); return Task.FromResult(receipt); }
    public Task<IEnumerable<GoodsReceipt>> GetReceiptsAsync(Guid? purchaseOrderId = null) => Task.FromResult(purchaseOrderId.HasValue ? _receipts.Where(r => r.PurchaseOrderId == purchaseOrderId.Value) : _receipts.AsEnumerable());

    public Task<ProcurementStatistics> GetStatisticsAsync(string? department = null)
    {
        var reqs = string.IsNullOrEmpty(department) ? _requisitions : _requisitions.Where(r => r.Department == department).ToList();
        var orders = string.IsNullOrEmpty(department) ? _orders : _orders.Where(o => o.Department == department).ToList();
        return Task.FromResult(new ProcurementStatistics { TotalRequisitions = reqs.Count, PendingRequisitions = reqs.Count(r => r.Status == RequisitionStatus.Submitted), TotalPurchaseOrders = orders.Count, OpenPurchaseOrders = orders.Count(o => o.Status == PurchaseOrderStatus.Approved || o.Status == PurchaseOrderStatus.Sent), TotalPOValue = orders.Sum(o => o.TotalAmount), ActiveTenders = _tenders.Count(t => t.Status == TenderStatus.Open), TotalBids = _bids.Count, OrdersByStatus = orders.GroupBy(o => o.Status.ToString()).ToDictionary(g => g.Key, g => g.Count()), SpendByDepartment = orders.GroupBy(o => o.Department).ToDictionary(g => g.Key, g => g.Sum(o => o.TotalAmount)) });
    }
}
