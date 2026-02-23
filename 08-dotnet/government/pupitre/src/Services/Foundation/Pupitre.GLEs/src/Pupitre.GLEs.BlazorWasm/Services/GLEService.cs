//using Mamey.Bank.Accounts.BlazorWasm.Models;
//using Mamey.Http;
//using Microsoft.Extensions.Logging;

//namespace Mamey.Bank.Accounts.BlazorWasm.Services;

//internal class GLEsService : IAccountsService
//{
//    private readonly ILogger<AccountsService> _logger;
//    private readonly IHttpClient _client;
//    private readonly IApiResponseHandler _apiResponseHandler;


//    public AccountsService(ILogger<AccountsService> logger, IHttpClient client,
//        IApiResponseHandler apiResponseHandler)
//    {
//        _logger = logger;
//        _client = client;
//        _apiResponseHandler = apiResponseHandler;
//    }

//    public async Task<List<Account>> GetAsync()
//        => await _apiResponseHandler.HandleAsync(_client.GetAsync<List<Account>>($"/accounts"));

//    public async Task<AccountDetails> GetAsync(Guid bankaccountId)
//        => await _apiResponseHandler.HandleAsync(_client.GetAsync<AccountDetails>($"/accounts/{bankaccountId}"));

    
//}


