namespace Mamey.Mifos.Services
{
    public interface IMifosSavingsAccountFixedDepositsService
    {
		Task GetAsync();
		Task SubmitApplicationAsync(int accountId);
		Task RetrieveAsync(int accountId);
		Task ModifiyAsync(int accountId);
		Task DeleteAsync(int accountId);
		Task ApproveApplicationAsync(int accountId);
		Task UndoApplicationApprovalAsync(int accountId);
		Task RejectApplicationAsync(int accountId);
		Task WithdrawApplicationAsync(int accountId);
		Task ActivateAccountAsync(int accountId);
		Task CloseAccountAsync(int accountId);
		Task PrematureCloseAccountAsync(int accountId);
		Task CalculatePrematureAmountAsync(int accountId);
		Task CalculateInterestAsync(int accountId);
		Task PostInterestAsync(int accountId);
	}

	
}

