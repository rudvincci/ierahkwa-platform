public class PaymentMethodDetails
{
    public string Id { get; set; }
    public string Type { get; set; }
    public CardDetails Card { get; set; }
    public BankAccountDetails BankAccount { get; set; }
    // Other payment method types like wallets or direct debits can be added here
    
    public Dictionary<string, object> Metadata { get; set; }

    public PaymentMethodDetails()
    {
        Metadata = new Dictionary<string, object>();
    }
}

public class CardDetails
{
    public string Brand { get; set; }
    public string Country { get; set; }
    public int ExpMonth { get; set; }
    public int ExpYear { get; set; }
    public string Last4 { get; set; }
    // Additional card-specific properties
}

public class BankAccountDetails
{
    public string BankName { get; set; }
    public string Country { get; set; }
    public string Last4 { get; set; }
    // Additional bank account-specific properties
}
public class ShippingDetails
{
    public Mamey.Stripe.Models.Address ShippingAddress { get; set; }
    public string Name { get; set; }
    public string Carrier { get; set; }
    public string TrackingNumber { get; set; }
    // Additional shipping details
}
