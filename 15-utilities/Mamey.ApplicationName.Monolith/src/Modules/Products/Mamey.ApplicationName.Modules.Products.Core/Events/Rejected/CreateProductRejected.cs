using System;
using Mamey.CQRS.Events;

namespace Mamey.ApplicationName.Modules.Products.Core.Events.Rejected;

public class CreateProductRejected : IRejectedEvent
{
    public CreateProductRejected(string reason, Guid? ProductId = null)
    {
        Reason = reason;
        ProductId = ProductId;
    }
    public Guid? ProductId { get; }

    public string Reason { get; }

    public string Code => "create_product_rejected";
}
