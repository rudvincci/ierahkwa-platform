namespace Mamey.Mifos.Services
{
    public interface IMifosSavingsAccountRecurringDepositService
    {
		Task GetAsync();
		Task SubmitApplicationAsync(int accountId);
		Task RetrieveAsync(int accountId);
		Task ModifyApplicationAsync(int accountId);
		Task DeleteApplicationAsync(int accountId);
		Task ApproveApplicationAsync(int accountId);
		Task UndoApplicationApprovalAsync(int accountId);
		Task RejectApplicationAsync(int accountId);
		Task WithdrawApplicationAsync(int accountId);
		Task ActivateAccountAsync(int accountId);
		Task UpdateRecommendedDepositAmountAsync(int accountId);
		Task CloseAccountAsync(int accountId);
		Task PrematureCloseAccountAsync(int accountId);
		Task CalculatePrematureAmountAsync(int accountId);
		Task CalculateInterestAsync(int accountId);
		Task PostInterestAsync(int accountId);
		Task DepositAsync(int accountId);
		Task WithdrawAsync(int accountId);
		Task UndoTransactionAsync(int accountId, int transactionId);
		Task AdjustTransactionAsync(int accountId, int transactionId);
		Task RetrieveTransactionAsync(int accountId, int transactionId);
	}

	
}

