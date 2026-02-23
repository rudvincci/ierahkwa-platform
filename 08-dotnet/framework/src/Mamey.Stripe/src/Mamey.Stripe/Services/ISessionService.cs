//using Mamey.Stripe.Models;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface ISessionService
//    {
//        /// <summary>
//        /// Creates a new Checkout Session for collecting payment from a customer.
//        /// </summary>
//        /// <param name="request">Parameters for the Checkout Session creation, including payment details, customer information, and success or cancel redirection URLs.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
//        /// <returns>The created Session object with details necessary to direct the customer to the Checkout.</returns>
//        Task<Session> CreateAsync(SessionRequest request, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves details of an existing Checkout Session by its ID.
//        /// </summary>
//        /// <param name="sessionId">The unique identifier of the Session to retrieve.</param>
//        /// <returns>The requested Session object.</returns>
//        Task<Session> RetrieveAsync(string sessionId);

//        /// <summary>
//        /// Lists all Checkout Sessions, optionally filtered by parameters such as customer ID or session status.
//        /// </summary>
//        /// <param name="request">Parameters to filter the list of Sessions.</param>
//        /// <returns>A list of Session objects.</returns>
//        Task<IEnumerable<Session>> ListAsync(SessionListRequest request);
//    }
//}
//public class SessionService : ISessionService
//{
//    private readonly ILogger<SessionService> _logger;

//    public SessionService(ILogger<SessionService> logger)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//    }

//    public async Task<Session> CreateAsync(SessionRequest request, string idempotencyKey = null)
//    {
//        try
//        {
//            var options = new SessionCreateOptions
//            {
//                PaymentMethodTypes = new List<string> { "card" },
//                LineItems = request.LineItems,
//                SuccessUrl = request.SuccessUrl,
//                CancelUrl = request.CancelUrl,
//                // Additional configurations as needed
//            };
//            var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//            var service = new SessionService();
//            var session = await service.CreateAsync(options, requestOptions);
//            return session; // Convert Stripe Session to your Session model
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error creating Checkout Session with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//            throw new ApplicationException("Failed to create Checkout Session.", ex);
//        }
//    }

//    public async Task<Session> RetrieveAsync(string sessionId)
//    {
//        try
//        {
//            var service = new SessionService();
//            var session = await service.GetAsync(sessionId);
//            return session; // Convert Stripe Session to your Session model
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error retrieving Checkout Session {SessionId}", sessionId);
//            throw new ApplicationException($"Failed to retrieve Checkout Session with ID {sessionId}.", ex);
//        }
//    }

//    public async Task<IEnumerable<Session>> ListAsync(SessionListRequest request)
//    {
//        try
//        {
//            var options = new SessionListOptions
//            {
//                Limit = request.Limit,
//                // Additional filters as needed
//            };
//            var service = new SessionService();
//            StripeList<Session> sessions = await service.ListAsync(options);
//            return sessions; // Convert Stripe Session list to your Session model list
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error listing Checkout Sessions");
//            throw new ApplicationException("Failed to list Checkout Sessions.", ex);
//        }
//    }
//}