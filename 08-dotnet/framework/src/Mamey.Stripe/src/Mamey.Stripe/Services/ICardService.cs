//using Mamey.Stripe.Models;
//using Mamey.Stripe.Services;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface ICardService
//    {
//        /// <summary>
//        /// Adds a new card to a customer's account.
//        /// </summary>
//        /// <param name="customerId">The Stripe customer ID to which the card is added.</param>
//        /// <param name="cardRequest">Card details for the new card.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the add operation.</param>
//        /// <returns>The added Card object.</returns>
//        Task<Card> AddAsync(string customerId, CardRequest cardRequest, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves a card's details by its ID.
//        /// </summary>
//        /// <param name="customerId">The Stripe customer ID associated with the card.</param>
//        /// <param name="cardId">The unique identifier of the card to retrieve.</param>
//        /// <returns>The requested Card object.</returns>
//        Task<Card> RetrieveAsync(string customerId, string cardId);

//        /// <summary>
//        /// Updates an existing card with new information.
//        /// </summary>
//        /// <param name="customerId">The Stripe customer ID associated with the card.</param>
//        /// <param name="cardId">The unique identifier of the card to update.</param>
//        /// <param name="updateRequest">Updated card details.</param>
//        /// <returns>The updated Card object.</returns>
//        Task<Card> UpdateAsync(string customerId, string cardId, CardUpdateRequest updateRequest);

//        /// <summary>
//        /// Removes a card from a customer's account.
//        /// </summary>
//        /// <param name="customerId">The Stripe customer ID associated with the card.</param>
//        /// <param name="cardId">The unique identifier of the card to remove.</param>
//        /// <returns>A boolean indicating the success of the removal.</returns>
//        Task<bool> RemoveAsync(string customerId, string cardId);

//        /// <summary>
//        /// Lists all cards associated with a customer's account.
//        /// </summary>
//        /// <param name="customerId">The Stripe customer ID whose cards are to be listed.</param>
//        /// <returns>A list of Card objects.</returns>
//        Task<IEnumerable<Card>> ListAsync(string customerId);
//    }
//}
//public class CardService : ICardService
//    {
//        private readonly ILogger<CardService> _logger;

//        public CardService(ILogger<CardService> logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        //public async Task<Card> AddAsync(string customerId, CardRequest cardRequest, string idempotencyKey = null)
//        //{
//        //    try
//        //    {
//        //        var options = new CardCreateOptions
//        //        {
//        //            Source = new SourceCreateOptions
//        //            {
//        //                Type = SourceType.Card,
//        //                Token = cardRequest.Token // Assume cardRequest includes a Token property
//        //            }
//        //        };
//        //        var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//        //        var service = new CustomerService();
//        //        var card = await service.CreateSourceAsync(customerId, options, requestOptions);
//        //        return new Card { /* Map Stripe Card to your Card model */ };
//        //    }
//        //    catch (StripeException ex)
//        //    {
//        //        _logger.LogError(ex, "Error adding card to customer {CustomerId} with IdempotencyKey {IdempotencyKey}", customerId, idempotencyKey);
//        //        throw new ApplicationException("Failed to add card.", ex);
//        //    }
//        //}

//    public Task<IEnumerable<Card>> ListAsync(string customerId)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<bool> RemoveAsync(string customerId, string cardId)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<Card> RetrieveAsync(string customerId, string cardId)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<Card> UpdateAsync(string customerId, string cardId, CardUpdateRequest updateRequest)
//    {
//        throw new NotImplementedException();
//    }

//    // Implement other methods similarly, focusing on securely managing card details and handling exceptions

//}