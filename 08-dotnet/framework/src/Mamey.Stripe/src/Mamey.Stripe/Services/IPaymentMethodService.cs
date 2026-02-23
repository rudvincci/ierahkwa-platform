//using Mamey.Stripe.Models;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface IPaymentMethodService
//    {
//        /// <summary>
//        /// Creates a new payment method based on the provided details. This can include card details, bank account information, or other supported payment types.
//        /// </summary>
//        /// <param name="request">The payment method creation parameters.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
//        /// <returns>The created PaymentMethod object.</returns>
//        Task<PaymentMethod> CreateAsync(PaymentMethodRequest request, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves details of an existing payment method by its ID.
//        /// </summary>
//        /// <param name="paymentMethodId">The unique identifier of the payment method to retrieve.</param>
//        /// <returns>The requested PaymentMethod object.</returns>
//        Task<PaymentMethod> RetrieveAsync(string paymentMethodId);

//        /// <summary>
//        /// Updates an existing payment method with new information, such as billing details or card expiration.
//        /// </summary>
//        /// <param name="paymentMethodId">The ID of the payment method to update.</param>
//        /// <param name="updateRequest">Updated information for the payment method.</param>
//        /// <returns>The updated PaymentMethod object.</returns>
//        Task<PaymentMethod> UpdateAsync(string paymentMethodId, PaymentMethodUpdateRequest updateRequest);

//        /// <summary>
//        /// Attaches a payment method to a customer, making it available for future payments or subscriptions.
//        /// </summary>
//        /// <param name="paymentMethodId">The ID of the payment method to attach.</param>
//        /// <param name="customerId">The ID of the customer to whom the payment method will be attached.</param>
//        /// <returns>The attached PaymentMethod object, now associated with the specified customer.</returns>
//        Task<PaymentMethod> AttachAsync(string paymentMethodId, string customerId);

//        /// <summary>
//        /// Detaches a payment method from a customer, removing it from the list of available payment options.
//        /// </summary>
//        /// <param name="paymentMethodId">The ID of the payment method to detach.</param>
//        /// <returns>The detached PaymentMethod object, no longer associated with any customer.</returns>
//        Task<PaymentMethod> DetachAsync(string paymentMethodId);

//        /// <summary>
//        /// Lists all payment methods available to a customer, optionally filtered by type.
//        /// </summary>
//        /// <param name="customerId">The ID of the customer whose payment methods are to be listed.</param>
//        /// <param name="type">Optional filter for the type of payment methods to list (e.g., 'card').</param>
//        /// <returns>A list of PaymentMethod objects.</returns>
//        Task<IEnumerable<PaymentMethod>> ListAsync(string customerId, string type = null);
//    }
//}
//public class PaymentMethodService : IPaymentMethodService
//    {
//        private readonly StripeClient _stripeClient;

//        public PaymentMethodService(StripeClient stripeClient)
//        {
//            _stripeClient = stripeClient;
//        }

//        public async Task<PaymentMethod> CreateAsync(PaymentMethodRequest request, string idempotencyKey = null)
//        {
//            var options = new PaymentMethodCreateOptions
//            {
//                Type = request.Type,
//                Card = new PaymentMethodCardOptions
//                {
//                    Number = request.Card.Number,
//                    ExpMonth = request.Card.ExpMonth,
//                    ExpYear = request.Card.ExpYear,
//                    Cvc = request.Card.Cvc,
//                },
//                // Add other payment method types as needed
//            };
//            var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//            var service = new PaymentMethodService(_stripeClient);
//            var paymentMethod = await service.CreateAsync(options, requestOptions);
//            return paymentMethod; // Convert to your PaymentMethod model
//        }

//        public async Task<PaymentMethod> RetrieveAsync(string paymentMethodId)
//        {
//            var service = new PaymentMethodService(_stripeClient);
//            var paymentMethod = await service.GetAsync(paymentMethodId);
//            return paymentMethod; // Convert to your PaymentMethod model
//        }

//        public async Task<PaymentMethod> UpdateAsync(string paymentMethodId, PaymentMethodUpdateRequest updateRequest)
//        {
//            var options = new PaymentMethodUpdateOptions
//            {
//                // Update fields as needed
//            };
//            var service = new PaymentMethodService(_stripeClient);
//            var paymentMethod = await service.UpdateAsync(paymentMethodId, options);
//            return paymentMethod; // Convert to your PaymentMethod model
//        }

//        public async Task<PaymentMethod> AttachAsync(string paymentMethodId, string customerId)
//        {
//            var options = new PaymentMethodAttachOptions
//            {
//                Customer = customerId,
//            };
//            var service = new PaymentMethodService(_stripeClient);
//            var paymentMethod = await service.AttachAsync(paymentMethodId, options);
//            return paymentMethod; // Convert to your PaymentMethod model
//        }

//        public async Task<PaymentMethod> DetachAsync(string paymentMethodId)
//        {
//            var service = new PaymentMethodService(_stripeClient);
//            var paymentMethod = await service.DetachAsync(paymentMethodId);
//            return paymentMethod; // Convert to your PaymentMethod model
//        }

//        public async Task<IEnumerable<PaymentMethod>> ListAsync(string customerId, string type = null)
//        {
//            var options = new PaymentMethodListOptions
//            {
//                Customer = customerId,
//                Type = type,
//            };
//            var service = new PaymentMethodService(_stripeClient);
//            StripeList<PaymentMethod> paymentMethods = await service.ListAsync(options);
//            return paymentMethods; // Convert to your list of PaymentMethod models
//        }
//    }