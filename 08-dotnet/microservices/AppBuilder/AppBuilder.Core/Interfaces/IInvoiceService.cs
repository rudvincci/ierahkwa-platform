using AppBuilder.Core.Models;

namespace AppBuilder.Core.Interfaces;

/// <summary>Invoices - list, PDF/HTML for print. Appy: Professional PDF invoices.</summary>
public interface IInvoiceService
{
    IReadOnlyList<Invoice> GetByUser(string userId);
    Invoice? GetById(string id);
    string GetInvoiceHtml(Invoice inv, User? user);
}
