using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IFutureWampumPayClient
{
    Task<List<PaymentInfo>> GetP2PPaymentsAsync(int limit = 50);
    Task<PaymentInfo?> GetP2PPaymentAsync(string paymentId);
    Task<List<MerchantPaymentInfo>> GetMerchantPaymentsAsync(int limit = 50);
    Task<MerchantPaymentInfo?> GetMerchantPaymentAsync(string paymentId);
    Task<List<DisbursementInfo>> GetDisbursementsAsync(int limit = 50);
    Task<DisbursementInfo?> GetDisbursementAsync(string disbursementId);
    Task<List<RecurringPaymentInfo>> GetRecurringPaymentsAsync(int limit = 50);
    Task<List<MultisigPaymentInfo>> GetMultisigPaymentsAsync(int limit = 50);
    Task<MultisigPaymentInfo?> GetMultisigPaymentAsync(string paymentId);
    Task<List<PaymentWalletInfo>> GetWalletsAsync();
    Task<PaymentWalletInfo?> GetWalletAsync(string walletId);
}

