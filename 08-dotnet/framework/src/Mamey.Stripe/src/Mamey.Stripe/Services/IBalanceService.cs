//using Mamey.Stripe.Models;
//using System.Threading.Tasks;

//namespace Mamey.Stripe.Services
//{
//    public interface IBalanceService
//    {
//        /// <summary>
//        /// Retrieves the current balance of the Stripe account.
//        /// </summary>
//        /// <returns>The current account balance.</returns>
//        Task<Balance> RetrieveAsync();

//        /// <summary>
//        /// Retrieves the balance history, which includes transactions such as charges, refunds, and payouts.
//        /// </summary>
//        /// <param name="request">Parameters to filter the balance history, such as date range or transaction type.</param>
//        /// <returns>A list of balance transaction objects.</returns>
//        Task<IEnumerable<BalanceTransaction>> ListTransactionsAsync(BalanceTransactionListRequest request);
//    }
//}
//public class BalanceService : IBalanceService
//    {
//        private readonly ILogger<BalanceService> _logger;

//        public BalanceService(ILogger<BalanceService> logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task<Balance> RetrieveAsync()
//        {
//            try
//            {
//                var service = new BalanceService();
//                var stripeBalance = await service.GetAsync();
//                return new Balance
//                {
//                    Available = stripeBalance.Available, // Map relevant fields
//                    Pending = stripeBalance.Pending,
//                    // Map additional fields as necessary
//                };
//            }
//            catch (StripeException ex)
//            {
//                _logger.LogError(ex, "Error retrieving account balance.");
//                throw new ApplicationException("Failed to retrieve account balance.", ex);
//            }
//        }

//        public async Task<IEnumerable<BalanceTransaction>> ListTransactionsAsync(BalanceTransactionListRequest request)
//        {
//            try
//            {
//                var options = new BalanceTransactionListOptions
//                {
//                    Limit = request.Limit,
//                    AvailableOn = request.AvailableOn,
//                    Created = request.Created,
//                    Currency = request.Currency,
//                    Type = request.Type,
//                    // Additional filters based on BalanceTransactionListRequest
//                };
//                var service = new BalanceTransactionService();
//                StripeList<BalanceTransaction> transactions = await service.ListAsync(options);
//                return MapStripeTransactionsToBalanceTransactions(transactions); // Implement this mapping based on your model
//            }
//            catch (StripeException ex)
//            {
//                _logger.LogError(ex, "Error listing balance transactions.");
//                throw new ApplicationException("Failed to list balance transactions.", ex);
//            }
//        }

//        // Example mapping method - implement according to your models
//        private IEnumerable<BalanceTransaction> MapStripeTransactionsToBalanceTransactions(StripeList<BalanceTransaction> stripeTransactions)
//        {
//            var transactions = new List<BalanceTransaction>();
//            foreach (var stripeTransaction in stripeTransactions)
//            {
//                transactions.Add(new BalanceTransaction
//                {
//                    Id = stripeTransaction.Id,
//                    Amount = stripeTransaction.Amount,
//                    Currency = stripeTransaction.Currency,
//                    Type = stripeTransaction.Type,
//                    // Map additional fields as needed
//                });
//            }
//            return transactions;
//        }
//    }