using AppBuilder.Core.Models;

namespace AppBuilder.Core.Interfaces;

/// <summary>PayPal, Bank Transfer. Appy: Start accepting payments from day one.</summary>
public interface IPaymentService
{
    string CreatePayPalSubscription(string userId, string planId, string returnUrl, string cancelUrl);
    (bool Ok, string? Error) ProcessPayPalCallback(string paymentId, string userId, string planId);
    Invoice CreateBankTransferInvoice(string userId, decimal amount, string description);
    Invoice? GetInvoice(string id);
    IReadOnlyList<Invoice> GetUserInvoices(string userId);
}
