//using Mamey.Stripe.Models;
//using Mamey.Stripe.Services;

//public class AccountService : IAccountService
//{
//    private readonly ILogger<AccountService> _logger;

//    public AccountService(ILogger<AccountService> logger)
//    {
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//    }

//    //public async Task<Account> CreateAsync(AccountRequest request, string idempotencyKey = null)
//    //{
//    //    try
//    //    {
//    //        var options = new AccountCreateOptions
//    //        {
//    //            Type = request.Type,
//    //            Country = request.Country,
//    //            Email = request.Email,
//    //            // Additional options based on AccountRequest
//    //        };
//    //        var requestOptions = new RequestOptions { IdempotencyKey = idempotencyKey };
//    //        var service = new AccountService();
//    //        var account = await service.CreateAsync(options, requestOptions);
//    //        return new Account { /* Map Stripe Account to your Account model */ };
//    //    }
//    //    catch (StripeException ex)
//    //    {
//    //        _logger.LogError(ex, "Error creating account with IdempotencyKey {IdempotencyKey}", idempotencyKey);
//    //        throw new ApplicationException("Failed to create account.", ex);
//    //    }
//    //}

//    //public async Task<Account> RetrieveAsync(string accountId)
//    //{
//    //    try
//    //    {
//    //        var service = new AccountService();
//    //        var account = await service.GetAsync(accountId);
//    //        return new Account { /* Map Stripe Account to your Account model */ };
//    //    }
//    //    catch (StripeException ex)
//    //    {
//    //        _logger.LogError(ex, "Error retrieving account {AccountId}", accountId);
//    //        throw new ApplicationException($"Failed to retrieve account with ID {accountId}.", ex);
//    //    }
//    //}

//    //public async Task<Account> UpdateAsync(string accountId, AccountUpdateRequest request)
//    //{
//    //    try
//    //    {
//    //        var options = new AccountUpdateOptions
//    //        {
//    //            // Map updated fields from AccountUpdateRequest to options
//    //        };
//    //        var service = new AccountService();
//    //        var account = await service.UpdateAsync(accountId, options);
//    //        return new Account { /* Map Stripe Account to your Account model */ };
//    //    }
//    //    catch (StripeException ex)
//    //    {
//    //        _logger.LogError(ex, "Error updating account {AccountId}", accountId);
//    //        throw new ApplicationException($"Failed to update account with ID {accountId}.", ex);
//    //    }
//    //}

//    //public async Task<bool> DeleteAsync(string accountId)
//    //{
//    //    try
//    //    {
//    //        var service = new AccountService();
//    //        var response = await service.DeleteAsync(accountId);
//    //        return response.Deleted;
//    //    }
//    //    catch (StripeException ex)
//    //    {
//    //        _logger.LogError(ex, "Error deleting account {AccountId}", accountId);
//    //        throw new ApplicationException($"Failed to delete account with ID {accountId}.", ex);
//    //    }
//    //}

//    //public async Task<IEnumerable<Account>> ListAsync(AccountListRequest request)
//    //{
//    //    try
//    //    {
//    //        var options = new AccountListOptions
//    //        {
//    //            Limit = request.Limit,
//    //            // Additional filters as needed
//    //        };
//    //        var service = new AccountService();
//    //        StripeList<Account> accounts = await service.ListAsync(options);
//    //        return new List<Account> { /* Map Stripe Accounts to your list of Account models */ };
//    //    }
//    //    catch (StripeException ex)
//    //    {
//    //        _logger.LogError(ex, "Error listing accounts");
//    //        throw new ApplicationException("Failed to list accounts.", ex);
//    //    }
//    //}
//}