//using Mamey.Stripe.Models;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface ITokenService
//    {
//        /// <summary>
//        /// Creates a token based on payment information, such as credit card details.
//        /// This token can then be used in place of the actual payment details for transactions.
//        /// </summary>
//        /// <param name="paymentInformation">The payment information to be tokenized.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the token creation.</param>
//        /// <returns>The created Token object containing the token id.</returns>
//        Task<Token> CreateAsync(PaymentInformation paymentInformation, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves details of an existing token by its ID. This can be used to verify the token's existence and status.
//        /// </summary>
//        /// <param name="tokenId">The unique identifier of the token to retrieve.</param>
//        /// <returns>The requested Token object.</returns>
//        Task<Token> RetrieveAsync(string tokenId);
//    }
//}
//public class TokenService : ITokenService
//{
//    private readonly ILogger<TokenService> _logger;

//    public TokenService(ILogger<TokenService> logger)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//    }

//    public async Task<Token> CreateAsync(PaymentInformation paymentInformation, string idempotencyKey = null)
//    {
//        try
//        {
//            var service = new TokenService();
//            var token = await service.CreateAsync(new TokenCreateOptions
//            {
//                Card = new TokenCardOptions
//                {
//                    Number = paymentInformation.CardNumber,
//                    ExpMonth = paymentInformation.ExpiryMonth,
//                    ExpYear = paymentInformation.ExpiryYear,
//                    Cvc = paymentInformation.CVC
//                }
//            }, new RequestOptions { IdempotencyKey = idempotencyKey });

//            return new Token { Id = token.Id }; // Assuming your Token model has an Id property
//        }
//        catch (StripeException ex)
//        {
//            _logger.LogError(ex, "Error creating token with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//            throw new ApplicationException("Failed to create token.", ex);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "An unexpected error occurred while creating a token.");
//            throw new ApplicationException("An unexpected error occurred while creating a token.", ex);
//        }
//    }

//    public async Task<Token> RetrieveAsync(string tokenId)
//    {
//        try
//        {
//            var service = new TokenService();
//            var token = await service.GetAsync(tokenId);
//            return new Token { Id = token.Id }; // Assuming your Token model has an Id property
//        }
//        catch (StripeException ex)
//        {
//            _logger.LogError(ex, "Error retrieving token {TokenId}", tokenId);
//            throw new ApplicationException($"Failed to retrieve token with ID {tokenId}.", ex);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "An unexpected error occurred while retrieving a token.");
//            throw new ApplicationException("An unexpected error occurred while retrieving a token.", ex);
//        }
//    }
//}