using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;

namespace AppBuilder.Infrastructure.Services;

/// <summary>Invoices – list, HTML for print/PDF. IERAHKWA Appy: Professional invoices.</summary>
public class InvoiceService : IInvoiceService
{
    private readonly IPaymentService _payment;
    private readonly IAuthService _auth;

    public InvoiceService(IPaymentService payment, IAuthService auth)
    {
        _payment = payment;
        _auth = auth;
    }

    public IReadOnlyList<Invoice> GetByUser(string userId) => _payment.GetUserInvoices(userId);
    public Invoice? GetById(string id) => _payment.GetInvoice(id);

    public string GetInvoiceHtml(Invoice inv, User? user)
    {
        var u = user ?? _auth.GetUserById(inv.UserId);
        var name = u?.Name ?? u?.Email ?? "Customer";
        var email = u?.Email ?? "";
        return $@"<!DOCTYPE html><html><head><meta charset=""utf-8""/><title>Invoice {inv.Number}</title><style>
body{{font-family:Segoe UI,sans-serif;max-width:700px;margin:2rem auto;padding:1rem;color:#1a1a1a;}}
h1{{color:#1a237e;border-bottom:2px solid #ffd700;}}
table{{width:100%;border-collapse:collapse;margin:1rem 0;}}
th,td{{border:1px solid #ddd;padding:8px;text-align:left;}}
th{{background:#1a237e;color:#fff;}}
.footer{{margin-top:2rem;font-size:0.9rem;color:#666;}}
</style></head><body>
<h1>IERAHKWA Appy – Invoice</h1>
<p><strong>Invoice:</strong> {inv.Number} &nbsp; <strong>Date:</strong> {inv.CreatedAt:yyyy-MM-dd}</p>
<p><strong>Bill to:</strong><br/>{name}<br/>{email}</p>
<table>
<tr><th>Description</th><th>Amount</th></tr>
<tr><td>{System.Net.WebUtility.HtmlEncode(inv.Description ?? "Subscription")}</td><td>{inv.Currency} {inv.Amount:N2}</td></tr>
</table>
<p><strong>Status:</strong> {inv.Status} &nbsp; <strong>Payment:</strong> {inv.PaymentMethod}</p>
<div class=""footer"">Sovereign Government of Ierahkwa Ne Kanienke · Ierahkwa Futurehead Appy · © 2026</div>
</body></html>";
    }
}
