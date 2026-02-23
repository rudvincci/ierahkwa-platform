using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IFutureWampumMerchantClient
{
    Task<List<MerchantOnboardingInfo>> GetMerchantOnboardingsAsync();
    Task<List<MerchantSettlementPaymentInfo>> GetMerchantPaymentsAsync();
    Task<List<SettlementInfo>> GetSettlementsAsync();
    Task<List<InvoiceInfo>> GetInvoicesAsync();
    Task<List<QRCodeInfo>> GetQRCodesAsync();
    Task<MerchantAnalyticsInfo?> GetMerchantAnalyticsAsync(string merchantId, string period);
    Task<List<MerchantComplianceInfo>> GetMerchantComplianceChecksAsync();
}

