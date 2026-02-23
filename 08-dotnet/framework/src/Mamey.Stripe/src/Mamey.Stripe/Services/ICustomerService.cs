//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Mamey.Stripe.Models;

//namespace Mamey.Stripe.Services;

//public interface ICustomerService
//{
//    /// <summary>
//    /// Creates a new customer in Stripe.
//    /// </summary>
//    /// <param name="request">Parameters for creating a new customer.</param>
//    /// <param name="idempotencyKey">A unique key to ensure idempotency of the create operation.</param>
//    /// <returns>The created Customer object.</returns>
//    Task<Customer> CreateAsync(CustomerRequest request, string idempotencyKey = null);

//    /// <summary>
//    /// Retrieves a customer by their Stripe ID.
//    /// </summary>
//    /// <param name="customerId">The unique identifier of the customer to retrieve.</param>
//    /// <returns>The requested Customer object.</returns>
//    Task<Customer> RetrieveAsync(string customerId);

//    /// <summary>
//    /// Updates an existing customer with new information.
//    /// </summary>
//    /// <param name="customerId">The unique identifier of the customer to update.</param>
//    /// <param name="request">Parameters for updating the customer.</param>
//    /// <returns>The updated Customer object.</returns>
//    Task<Customer> UpdateAsync(string customerId, CustomerUpdateRequest request);

//    /// <summary>
//    /// Deletes a customer from Stripe. Note that deleting a customer is irreversible.
//    /// </summary>
//    /// <param name="customerId">The unique identifier of the customer to delete.</param>
//    /// <returns>A boolean indicating the success of the operation.</returns>
//    Task<bool> DeleteAsync(string customerId);

//    /// <summary>
//    /// Lists all customers with optional filtering parameters.
//    /// </summary>
//    /// <param name="request">Parameters to filter the list of customers.</param>
//    /// <returns>A list of Customer objects.</returns>
//    Task<IEnumerable<Customer>> ListAsync(CustomerListRequest request);

//    /// <summary>
//    /// Adds a payment method to a customer's account. This is useful for setting up future payments.
//    /// </summary>
//    /// <param name="customerId">The unique identifier of the customer.</param>
//    /// <param name="paymentMethodId">The ID of the payment method to attach.</param>
//    /// <param name="idempotencyKey">A unique key to ensure idempotency of the operation.</param>
//    /// <returns>The updated Customer object.</returns>
//    Task<Customer> AddPaymentMethodAsync(string customerId, string paymentMethodId, string idempotencyKey = null);

//    /// <summary>
//    /// Handles edge cases when adding a payment method fails, such as due to network issues or payment method decline.
//    /// </summary>
//    /// <param name="customerId">The unique identifier of the customer.</param>
//    /// <param name="error">The error encountered when adding the payment method.</param>
//    /// <returns>Guidance on resolving the issue, such as retrying or using an alternative payment method.</returns>
//    Task<PaymentMethodAdditionResult> HandlePaymentMethodAdditionFailureAsync(string customerId, string error);
//}

//public class PaymentMethodAdditionResult
//{
//    public bool RetryRecommended { get; set; }
//    public string AlternativePaymentMethodRecommended { get; set; }
//    // Additional properties and recommendations based on the error encountered
//}

