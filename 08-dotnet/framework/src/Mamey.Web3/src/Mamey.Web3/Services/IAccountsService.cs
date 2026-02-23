using DynamicData;
using Mamey.Web3.Model;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Mamey.Web3.Services;

public interface IAccountsService
{
    IEnumerable<string> GetAccountsAddresses();
    Task<decimal> GetAccountEtherBalanceAsync(string address);
    SourceCache<AccountInfo, string> Accounts { get; set; }
    void AddAccount(AccountInfo accountInfo, string privateKey);
    Task<string> SendTransactionAsync(TransactionInput transactionInput);
    Task<string> SendTransactionAsync<TFunctionMessage>(string contractAddress, TFunctionMessage functionMessage) where TFunctionMessage : FunctionMessage, new();
    List<TransactionInfo> GetCurrentChainPendingTransactions();
    void UpdateTransactionInfo(TransactionInfo transactionInfo);
    Task<decimal> GetAccountTokenBalanceAsync(string address, string contractAddress, int numberOfDecimals = 18);
}