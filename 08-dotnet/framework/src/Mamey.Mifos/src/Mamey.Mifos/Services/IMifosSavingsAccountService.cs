namespace Mamey.Mifos.Services
{
    public interface IMifosSavingsAccountService
	{
		Task GetAsync();
		Task RetrieveAsync();
		Task SubmitApplicationAsync();

		Task ModifyApplicationAsync(int accountId);
		Task DeleteApplicationAsync(int accountId);
		Task ModifyApplicationWithholdTaskAsync(int accountId);

		Task ApproveApplicationAsync(int accountId);
		Task UndoApplicationApprovalAsync(int accountId);
		Task RejectApplicationAsync(int accountId);
		Task WithdrawApplicationAsync(int accountId);
		Task ActivateAccountAsync(int accountId);
		Task CloseAccountAsync(int accountId);
		Task CalculateInterestAsync(int accountId);
		Task PostInterestAsync(int accountId);
		Task BlockAsync(int accountId);
		Task UnblockAsync(int accountId);
		Task BlockCreditTransactionsAsync(int accountId);
		Task UnblockCreditTransactionsAsync(int accountId);
		Task BlockDebitTransactionsAsync(int accountId);
		Task UnblockDebitTransactionsAsync(int accountId);

		Task SubmitGSIMAsync(int gismId);
		Task ApproveGSIMAsync(int gismId);
		Task WithdrawGSIMAsync(int gismId);
		Task CloseGSIMAsync(int gismId);

		Task MakeDepositAsync(int accountId);
		Task MakeGSIMDepositAsync(int accountId);
		Task MakeWithdrawlAsync(int accountId);
		Task UndoTransactionAsync(int accountId, int transactionId);
		Task AdjustTransactionAsync(int accountId, int transactionId);
		Task HoldAmountAsync(int accountId, int transactionId);
		Task ReleaseAmountAsync(int accountId, int transactionId);
		Task RetrieveTransactionAsync(int accountId, int transactionId);
	}
}

