//namespace Mamey.Stripe.Services;
//public interface IPaymentIntentService
//{
//    /// <summary>
//    /// Creates a PaymentIntent object.
//    /// </summary>
//    /// <param name="request">PaymentIntent creation request parameters.</param>
//    /// <returns>The created PaymentIntent.</returns>
//    Task<PaymentIntent> CreateAsync(PaymentIntentRequest request, string idempotencyKey);

//    /// <summary>
//    /// Retrieves a PaymentIntent object.
//    /// </summary>
//    /// <param name="paymentIntentId">The ID of the PaymentIntent to retrieve.</param>
//    /// <returns>The requested PaymentIntent.</returns>
//    Task<PaymentIntent> RetrieveAsync(string paymentIntentId);

//    /// <summary>
//    /// Updates a PaymentIntent object.
//    /// </summary>
//    /// <param name="paymentIntentId">The ID of the PaymentIntent to update.</param>
//    /// <param name="request">PaymentIntent update request parameters.</param>
//    /// <returns>The updated PaymentIntent.</returns>
//    Task<PaymentIntent> UpdateAsync(string paymentIntentId, PaymentIntentUpdateRequest request);

//    /// <summary>
//    /// Confirms a PaymentIntent object.
//    /// </summary>
//    /// <param name="paymentIntentId">The ID of the PaymentIntent to confirm.</param>
//    /// <returns>The confirmed PaymentIntent.</returns>
//    Task<PaymentIntent> ConfirmAsync(string paymentIntentId, PaymentIntentConfirmRequest request, string idempotencyKey);

//    /// <summary>
//    /// Cancels a PaymentIntent object.
//    /// </summary>
//    /// <param name="paymentIntentId">The ID of the PaymentIntent to cancel.</param>
//    /// <returns>The canceled PaymentIntent.</returns>
//    Task<PaymentIntent> CancelAsync(string paymentIntentId, PaymentIntentCancelRequest request);

//    /// <summary>
//    /// Lists PaymentIntent objects.
//    /// </summary>
//    /// <param name="request">PaymentIntent list request parameters.</param>
//    /// <returns>A list of PaymentIntents.</returns>
//    Task<IEnumerable<PaymentIntent>> ListAsync(PaymentIntentListRequest request);

//    /// <summary>
//    /// Captures a PaymentIntent object for manual capture flows.
//    /// </summary>
//    /// <param name="paymentIntentId">The ID of the PaymentIntent to capture.</param>
//    /// <returns>The captured PaymentIntent.</returns>
//    Task<PaymentIntent> CaptureAsync(string paymentIntentId, PaymentIntentCaptureRequest request);

//    Task<PaymentIntent> HandleFailedPaymentAsync(string paymentIntentId);
//    Task<PaymentMethod> AttachPaymentMethodAsync(string paymentIntentId, string paymentMethodId);
//    Task<bool> DetachPaymentMethodAsync(string paymentMethodId);
//    Task<PaymentIntent> CreateOffSessionAsync(PaymentIntentRequest request);
//    Task<PaymentIntent> RetryOffSessionPaymentAsync(string paymentIntentId);
//    Task<PaymentIntent> CreateWithCurrencyConversionAsync(PaymentIntentRequest request, string targetCurrency);
//    Task<PaymentIntent> UpdateAmountAsync(string paymentIntentId, long newAmount);

//}
//public class PaymentIntentService : IPaymentIntentService
//{
//    private readonly StripeClient _stripeClient;

//    public PaymentIntentService(StripeClient stripeClient)
//    {
//        _stripeClient = stripeClient ?? throw new ArgumentNullException(nameof(stripeClient));
//    }

//    public async Task<PaymentIntent> CreateAsync(PaymentIntentRequest request, string idempotencyKey)
//    {
//        var options = new PaymentIntentCreateOptions
//        {
//            Amount = request.Amount,
//            Currency = request.Currency,
//            // Map other fields from request to options
//        };
//        var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.CreateAsync(options, requestOptions);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//    public async Task<PaymentIntent> RetrieveAsync(string paymentIntentId)
//    {
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.GetAsync(paymentIntentId);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//    public async Task<PaymentIntent> UpdateAsync(string paymentIntentId, PaymentIntentUpdateRequest request)
//    {
//        var options = new PaymentIntentUpdateOptions
//        {
//            // Map fields from request to options
//        };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.UpdateAsync(paymentIntentId, options);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//    public async Task<PaymentIntent> ConfirmAsync(string paymentIntentId, PaymentIntentConfirmRequest request, string idempotencyKey)
//    {
//        var options = new PaymentIntentConfirmOptions
//        {
//            // Additional confirmation options can be set here
//        };
//        var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.ConfirmAsync(paymentIntentId, options, requestOptions);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//    public async Task<PaymentIntent> CancelAsync(string paymentIntentId, PaymentIntentCancelRequest request)
//    {
//        var options = new PaymentIntentCancelOptions
//        {
//            // Cancellation specific options can be set here
//        };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.CancelAsync(paymentIntentId, options);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//    public async Task<IEnumerable<PaymentIntent>> ListAsync(PaymentIntentListRequest request)
//    {
//        var options = new PaymentIntentListOptions
//        {
//            Limit = request.Limit,
//            // Additional filtering options can be set here
//        };
//        var service = new PaymentIntentService(_stripeClient);
//        StripeList<PaymentIntent> paymentIntents = await service.ListAsync(options);
//        return paymentIntents; // Convert to your list of PaymentIntent models
//    }
//    public async Task<PaymentIntent> ConfirmAsync(string paymentIntentId, PaymentIntentConfirmRequest request, string idempotencyKey)
//    {
//        var options = new PaymentIntentConfirmOptions
//        {
//            // Additional confirmation options can be set here
//        };
//        var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.ConfirmAsync(paymentIntentId, options, requestOptions);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//    public async Task<PaymentIntent> CancelAsync(string paymentIntentId, PaymentIntentCancelRequest request)
//    {
//        var options = new PaymentIntentCancelOptions
//        {
//            // Cancellation specific options can be set here
//        };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.CancelAsync(paymentIntentId, options);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//    public async Task<IEnumerable<PaymentIntent>> ListAsync(PaymentIntentListRequest request)
//    {
//        var options = new PaymentIntentListOptions
//        {
//            Limit = request.Limit,
//            // Additional filtering options can be set here
//        };
//        var service = new PaymentIntentService(_stripeClient);
//        StripeList<PaymentIntent> paymentIntents = await service.ListAsync(options);
//        return paymentIntents; // Convert to your list of PaymentIntent models
//    }
//    public async Task<PaymentIntent> CaptureAsync(string paymentIntentId, PaymentIntentCaptureRequest request)
//    {
//        var options = new PaymentIntentCaptureOptions
//        {
//            AmountToCapture = request.AmountToCapture,
//            // Additional capture options can be set here
//        };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.CaptureAsync(paymentIntentId, options);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//    public async Task<PaymentIntent> HandleFailedPaymentAsync(string paymentIntentId)
//    {
//        // Custom logic to handle a failed payment intent
//        // This could involve sending notifications, updating the database, etc.
//        throw new NotImplementedException();
//    }

//    public async Task<PaymentMethod> AttachPaymentMethodAsync(string paymentIntentId, string paymentMethodId)
//    {
//        var options = new PaymentMethodAttachOptions
//        {
//            Customer = paymentIntentId,
//        };
//        var service = new PaymentMethodService(_stripeClient);
//        var paymentMethod = await service.AttachAsync(paymentMethodId, options);
//        return paymentMethod; // Convert to your PaymentMethod model
//    }

//    public async Task<bool> DetachPaymentMethodAsync(string paymentMethodId)
//    {
//        var service = new PaymentMethodService(_stripeClient);
//        var paymentMethod = await service.DetachAsync(paymentMethodId);
//        // Detaching a payment method doesn't delete it, but it's no longer associated with a customer
//        return paymentMethod != null;
//    }

//    public async Task<PaymentIntent> CreateOffSessionAsync(PaymentIntentRequest request)
//    {
//        var options = new PaymentIntentCreateOptions
//        {
//            Amount = request.Amount,
//            Currency = request.Currency,
//            Customer = request.CustomerId,
//            OffSession = true,
//            Confirm = true,
//            // Map other fields from request to options as needed
//        };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.CreateAsync(options);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }
//    public async Task<PaymentIntent> RetryOffSessionPaymentAsync(string paymentIntentId)
//    {
//        // Assuming that you have logic to determine whether the PaymentIntent can be retried
//        // and that it is set up for off-session payments.
//        var service = new PaymentIntentService(_stripeClient);
//        var options = new PaymentIntentConfirmOptions { };
//        var paymentIntent = await service.ConfirmAsync(paymentIntentId, options);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }
//    public async Task<PaymentIntent> CreateWithCurrencyConversionAsync(PaymentIntentRequest request, string targetCurrency)
//    {
//        // This example assumes that you have a way to convert the amount to the target currency.
//        // You might need to integrate with a currency conversion API or have predefined rates.
//        long convertedAmount = ConvertCurrency(request.Amount, request.Currency, targetCurrency);
        
//        var options = new PaymentIntentCreateOptions
//        {
//            Amount = convertedAmount,
//            Currency = targetCurrency,
//            // Copy other necessary fields from the original request
//        };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.CreateAsync(options);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//    private long ConvertCurrency(long amount, string fromCurrency, string toCurrency)
//    {
//        // Implement your currency conversion logic here
//        throw new NotImplementedException();
//    }
//    public async Task<PaymentIntent> UpdateAmountAsync(string paymentIntentId, long newAmount)
//    {
//        var options = new PaymentIntentUpdateOptions
//        {
//            Amount = newAmount,
//        };
//        var service = new PaymentIntentService(_stripeClient);
//        var paymentIntent = await service.UpdateAsync(paymentIntentId, options);
//        return paymentIntent; // Convert to your PaymentIntent model
//    }

//}
