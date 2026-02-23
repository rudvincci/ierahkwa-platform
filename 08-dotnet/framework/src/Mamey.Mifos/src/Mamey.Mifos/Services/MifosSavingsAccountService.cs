namespace Mamey.Mifos.Services
{
    public class MifosSavingsAccountService : IMifosSavingsAccountService
    {
        private readonly IMifosApiClient _mifosApiClient;

        public MifosSavingsAccountService(IMifosApiClient mifosApiClient)
        {
            _mifosApiClient = mifosApiClient;
        }

        public Task ActivateAccountAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task AdjustTransactionAsync(int accountId, int transactionId)
        {
            throw new NotImplementedException();
        }

        public Task ApproveApplicationAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task ApproveGSIMAsync(int gismId)
        {
            throw new NotImplementedException();
        }

        public Task BlockAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task BlockCreditTransactionsAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task BlockDebitTransactionsAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task CalculateInterestAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task CloseAccountAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task CloseGSIMAsync(int gismId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteApplicationAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task HoldAmountAsync(int accountId, int transactionId)
        {
            throw new NotImplementedException();
        }

        public Task MakeDepositAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task MakeGSIMDepositAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task MakeWithdrawlAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task ModifyApplicationAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task ModifyApplicationWithholdTaskAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task PostInterestAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task RejectApplicationAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task ReleaseAmountAsync(int accountId, int transactionId)
        {
            throw new NotImplementedException();
        }

        public Task RetrieveAsync()
        {
            throw new NotImplementedException();
        }

        public Task RetrieveTransactionAsync(int accountId, int transactionId)
        {
            throw new NotImplementedException();
        }

        public Task SubmitApplicationAsync()
        {
            throw new NotImplementedException();
        }

        public Task SubmitGSIMAsync(int gismId)
        {
            throw new NotImplementedException();
        }

        public Task UnblockAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task UnblockCreditTransactionsAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task UnblockDebitTransactionsAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task UndoApplicationApprovalAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task UndoTransactionAsync(int accountId, int transactionId)
        {
            throw new NotImplementedException();
        }

        public Task WithdrawApplicationAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task WithdrawGSIMAsync(int gismId)
        {
            throw new NotImplementedException();
        }
    }
}

