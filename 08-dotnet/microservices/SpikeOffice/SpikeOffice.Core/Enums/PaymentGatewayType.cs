namespace SpikeOffice.Core.Enums;

public enum PaymentGatewayType
{
    // Online
    Stripe = 1,
    PayPal = 2,
    Razorpay = 3,
    Mollie = 4,
    Paystack = 5,
    Flutterwave = 6,
    MercadoPago = 7,
    // Offline (dynamic)
    Cash = 100,
    BankTransfer = 101,
    MobileMoney = 102,
    Cheque = 103,
    Custom = 199
}
