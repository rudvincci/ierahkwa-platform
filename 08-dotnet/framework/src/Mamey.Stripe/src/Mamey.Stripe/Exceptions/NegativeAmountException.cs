using Mamey.Exceptions;

namespace Mamey.Stripe.Exceptions;

public class NegativeAmountException : MameyException
{
    public NegativeAmountException()
    {
    }
}

public class PaymentIntentMinimumAmountException : MameyException
{
    public PaymentIntentMinimumAmountException()
    {
    }
}
public class PaymentIntentMaximumAmountException : MameyException
{
    public PaymentIntentMaximumAmountException()
    {
    }
}