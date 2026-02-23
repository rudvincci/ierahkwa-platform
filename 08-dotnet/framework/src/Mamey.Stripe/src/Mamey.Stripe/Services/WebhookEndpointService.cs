//using Stripe;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services;

//public class WebhookEndpointService : IWebhookEndpointService
//{
//    private readonly ILogger<WebhookEndpointService> _logger;

//    public WebhookEndpointService(ILogger<WebhookEndpointService> logger)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//    }

//    public async Task<WebhookEndpoint> CreateAsync(WebhookEndpointRequest request, string idempotencyKey = null)
//    {
//        try
//        {
//            var options = new WebhookEndpointCreateOptions
//            {
//                Url = request.Url,
//                EnabledEvents = request.EnabledEvents,
//                Description = request.Description,
//            };
//            var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//            var service = new WebhookEndpointService();
//            var webhookEndpoint = await service.CreateAsync(options, requestOptions);
//            return new WebhookEndpoint { /* Map Stripe WebhookEndpoint to your WebhookEndpoint model */ };
//        }
//        catch (StripeException ex)
//        {
//            _logger.LogError(ex, "Error creating webhook endpoint with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//            throw new ApplicationException("Failed to create webhook endpoint.", ex);
//        }
//    }

//    public async Task<WebhookEndpoint> RetrieveAsync(string webhookEndpointId)
//    {
//        try
//        {
//            var service = new WebhookEndpointService();
//            var webhookEndpoint = await service.GetAsync(webhookEndpointId);
//            return new WebhookEndpoint { /* Map Stripe WebhookEndpoint to your WebhookEndpoint model */ };
//        }
//        catch (StripeException ex)
//        {
//            _logger.LogError(ex, "Error retrieving webhook endpoint {WebhookEndpointId}", webhookEndpointId);
//            throw new ApplicationException($"Failed to retrieve webhook endpoint with ID {webhookEndpointId}.", ex);
//        }
//    }

//    public async Task<WebhookEndpoint> UpdateAsync(string webhookEndpointId, WebhookEndpointUpdateRequest updateRequest)
//    {
//        try
//        {
//            var options = new WebhookEndpointUpdateOptions
//            {
//                Url = updateRequest.Url,
//                EnabledEvents = updateRequest.EnabledEvents,
//                Description = updateRequest.Description,
//            };
//            var service = new WebhookEndpointService();
//            var webhookEndpoint = await service.UpdateAsync(webhookEndpointId, options);
//            return new WebhookEndpoint { /* Map Stripe WebhookEndpoint to your WebhookEndpoint model */ };
//        }
//        catch (StripeException ex)
//        {
//            _logger.LogError(ex, "Error updating webhook endpoint {WebhookEndpointId}", webhookEndpointId);
//            throw new ApplicationException($"Failed to update webhook endpoint with ID {webhookEndpointId}.", ex);
//        }
//    }

//    public async Task<bool> DeleteAsync(string webhookEndpointId)
//    {
//        try
//        {
//            var service = new WebhookEndpointService();
//            var deleted = await service.DeleteAsync(webhookEndpointId);
//            return deleted.Deleted;
//        }
//        catch (StripeException ex)
//        {
//            _logger.LogError(ex, "Error deleting webhook endpoint {WebhookEndpointId}", webhookEndpointId);
//            throw new ApplicationException($"Failed to delete webhook endpoint with ID {webhookEndpointId}.", ex);
//        }
//    }

//    public async Task<IEnumerable<WebhookEndpoint>> ListAsync()
//    {
//        try
//        {
//            var service = new WebhookEndpointService();
//            StripeList<WebhookEndpoint> webhookEndpoints = await service.ListAsync();
//            return new List<WebhookEndpoint> { /* Map Stripe WebhookEndpoints to your list of WebhookEndpoint models */ };
//        }
//        catch (StripeException ex)
//        {
//            _logger.LogError(ex, "Error listing webhook endpoints");
//            throw new ApplicationException("Failed to list webhook endpoints.", ex);
//        }
//    }
//}