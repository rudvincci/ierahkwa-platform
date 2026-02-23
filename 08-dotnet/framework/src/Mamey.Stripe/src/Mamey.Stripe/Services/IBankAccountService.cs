//using Mamey.Stripe.Models;

//namespace Mamey.Stripe.Services
//{
//    public interface IBankAccountService
//    {
//        /// <summary>
//        /// Adds a new bank account to a Stripe account or a connected account.
//        /// </summary>
//        /// <param name="accountId">The Stripe account ID to which the bank account is added.</param>
//        /// <param name="bankAccountRequest">The bank account details.</param>
//        /// <param name="idempotencyKey">Unique key to ensure idempotency of the add operation.</param>
//        /// <returns>The added BankAccount object.</returns>
//        Task<BankAccount> AddAsync(string accountId, BankAccountRequest bankAccountRequest, string idempotencyKey = null);

//        /// <summary>
//        /// Retrieves details of a bank account associated with a Stripe account or connected account.
//        /// </summary>
//        /// <param name="accountId">The Stripe account ID.</param>
//        /// <param name="bankAccountId">The bank account ID to retrieve.</param>
//        /// <returns>The BankAccount object.</returns>
//        Task<BankAccount> RetrieveAsync(string accountId, string bankAccountId);

//        /// <summary>
//        /// Updates details of an existing bank account.
//        /// </summary>
//        /// <param name="accountId">The Stripe account ID.</param>
//        /// <param name="bankAccountId">The bank account ID to update.</param>
//        /// <param name="updateRequest">The updated bank account details.</param>
//        /// <returns>The updated BankAccount object.</returns>
//        Task<BankAccount> UpdateAsync(string accountId, string bankAccountId, BankAccountUpdateRequest updateRequest);

//        /// <summary>
//        /// Removes a bank account from a Stripe account or connected account.
//        /// </summary>
//        /// <param name="accountId">The Stripe account ID.</param>
//        /// <param name="bankAccountId">The bank account ID to remove.</param>
//        /// <returns>A boolean indicating the success of the removal.</returns>
//        Task<bool> RemoveAsync(string accountId, string bankAccountId);

//        /// <summary>
//        /// Lists all bank accounts associated with a Stripe account or connected account.
//        /// </summary>
//        /// <param name="accountId">The Stripe account ID.</param>
//        /// <returns>A list of BankAccount objects.</returns>
//        Task<IEnumerable<BankAccount>> ListAsync(string accountId);

//        /// <summary>
//        /// Verifies a bank account with Stripe, typically required for ACH payments and payouts.
//        /// </summary>
//        /// <param name="accountId">The Stripe account ID.</param>
//        /// <param name="bankAccountId">The bank account ID to verify.</param>
//        /// <param name="amounts">The amounts deposited to the bank account for verification purposes.</param>
//        /// <returns>The verified BankAccount object.</returns>
//        Task<BankAccount> VerifyAsync(string accountId, string bankAccountId, int[] amounts);
//    }
//    public class BankAccountService : IBankAccountService
//    {
//        private readonly ILogger<BankAccountService> _logger;

//        public BankAccountService(ILogger<BankAccountService> logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        //public async Task<BankAccount> AddAsync(string accountId, BankAccountRequest bankAccountRequest, string idempotencyKey = null)
//        //{
//        //    try
//        //    {
//        //        var options = new BankAccountCreateOptions
//        //        {
//        //            // Populate options based on bankAccountRequest
//        //        };
//        //        var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//        //        var service = new BankAccountService();
//        //        var bankAccount = await service.CreateAsync(accountId, options, requestOptions);
//        //        return new BankAccount { /* Map Stripe BankAccount to your BankAccount model */ };
//        //    }
//        //    catch (StripeException ex)
//        //    {
//        //        _logger.LogError(ex, "Error adding bank account to account {AccountId} with IdempotencyKey {IdempotencyKey}", accountId, idempotencyKey);
//        //        throw new ApplicationException("Failed to add bank account.", ex);
//        //    }
//        //}

//        public Task<IEnumerable<BankAccount>> ListAsync(string accountId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<bool> RemoveAsync(string accountId, string bankAccountId)
//        {
//            throw new NotImplementedException();
//        }

//        //public async Task<BankAccount> RetrieveAsync(string accountId, string bankAccountId)
//        //{
//        //    try
//        //    {
//        //        var service = new BankAccountService();
//        //        var bankAccount = await service.GetAsync(accountId, bankAccountId);
//        //        return new BankAccount { /* Map Stripe BankAccount to your BankAccount model */ };
//        //    }
//        //    catch (StripeException ex)
//        //    {
//        //        _logger.LogError(ex, "Error retrieving bank account {BankAccountId} for account {AccountId}", bankAccountId, accountId);
//        //        throw new ApplicationException($"Failed to retrieve bank account {bankAccountId}.", ex);
//        //    }
//        //}

//        public Task<BankAccount> UpdateAsync(string accountId, string bankAccountId, BankAccountUpdateRequest updateRequest)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<BankAccount> VerifyAsync(string accountId, string bankAccountId, int[] amounts)
//        {
//            throw new NotImplementedException();
//        }

//        // Implement other methods similarly, focusing on securely managing bank account details and handling exceptions

//    }
//}
