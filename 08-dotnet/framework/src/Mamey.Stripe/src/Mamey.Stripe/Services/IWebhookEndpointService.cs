//using Mamey.Stripe.Models;
//using Stripe;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services;

//public interface IWebhookEndpointService
//{
//    /// <summary>
//    /// Creates a new webhook endpoint for receiving event notifications from Stripe.
//    /// </summary>
//    /// <param name="request">Parameters for the webhook endpoint creation, including the URL, list of subscribed events, and description.</param>
//    /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
//    /// <returns>The created WebhookEndpoint object.</returns>
//    Task<WebhookEndpoint> CreateAsync(WebhookEndpointRequest request, string idempotencyKey = null);

//    /// <summary>
//    /// Retrieves details of an existing webhook endpoint by its ID.
//    /// </summary>
//    /// <param name="webhookEndpointId">The unique identifier of the webhook endpoint to retrieve.</param>
//    /// <returns>The requested WebhookEndpoint object.</returns>
//    Task<WebhookEndpoint> RetrieveAsync(string webhookEndpointId);

//    /// <summary>
//    /// Updates an existing webhook endpoint, allowing modifications to the URL, list of subscribed events, or other attributes.
//    /// </summary>
//    /// <param name="webhookEndpointId">The ID of the webhook endpoint to update.</param>
//    /// <param name="updateRequest">Updated information for the webhook endpoint.</param>
//    /// <returns>The updated WebhookEndpoint object.</returns>
//    Task<WebhookEndpoint> UpdateAsync(string webhookEndpointId, WebhookEndpointUpdateRequest updateRequest);

//    /// <summary>
//    /// Deletes a webhook endpoint, removing it from Stripe and stopping event notifications to that endpoint.
//    /// </summary>
//    /// <param name="webhookEndpointId">The ID of the webhook endpoint to delete.</param>
//    /// <returns>A boolean indicating the success of the deletion.</returns>
//    Task<bool> DeleteAsync(string webhookEndpointId);

//    /// <summary>
//    /// Lists all webhook endpoints, optionally filtered by specific criteria.
//    /// </summary>
//    /// <returns>A list of WebhookEndpoint objects.</returns>
//    Task<IEnumerable<WebhookEndpoint>> ListAsync();
//}
