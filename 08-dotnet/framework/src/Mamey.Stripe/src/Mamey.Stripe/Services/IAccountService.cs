using Mamey.Stripe.Models;

namespace Mamey.Stripe.Services;

public interface IAccountService
{
    /// <summary>
    /// Creates a new Stripe account, either standard or connected, based on the request parameters.
    /// </summary>
    /// <param name="request">Account creation parameters, including account type and capabilities.</param>
    /// <param name="idempotencyKey">Unique key to ensure idempotency of the create operation.</param>
    /// <returns>The created Account object.</returns>
    Task<Account> CreateAsync(AccountRequest request, string idempotencyKey = null);

    /// <summary>
    /// Retrieves details of an existing Stripe account by its ID.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account to retrieve.</param>
    /// <returns>The requested Account object.</returns>
    Task<Account> RetrieveAsync(string accountId);

    /// <summary>
    /// Updates an existing Stripe account with new information or settings.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account to update.</param>
    /// <param name="request">Account update parameters.</param>
    /// <returns>The updated Account object.</returns>
    Task<Account> UpdateAsync(string accountId, AccountUpdateRequest request);

    /// <summary>
    /// Deletes a Stripe account. This is typically restricted to connected accounts and may have various implications.
    /// </summary>
    /// <param name="accountId">The unique identifier of the account to delete.</param>
    /// <returns>A boolean indicating the success of the deletion.</returns>
    Task<bool> DeleteAsync(string accountId);

    /// <summary>
    /// Lists all Stripe accounts, either standard or connected, with optional filtering parameters.
    /// </summary>
    /// <param name="request">Parameters to filter the list of accounts.</param>
    /// <returns>A list of Account objects.</returns>
    Task<IEnumerable<Account>> ListAsync(AccountListRequest request);
}
