using Mamey.Mifos.Entities;
using Mamey.Mifos.Results;

namespace Mamey.Mifos.Services
{
    public interface IMifosLoansService
    {
        Task<LoanRepaymentScheduleResult> CalculateLoanRepaymentScheduleAsync();
        Task SubmitNewLoanApplicationAsync();
        Task<IEnumerable<Loan>> GetAsync();
        Task<Loan> GetAsync(int loanId);
        Task ApproveLoanApplicationAsync(int loanId, DateTime approvedOnDate,
            decimal? approvedLoanAmount = null, DateTime? expectedDisbursementDate = null);
        Task UndoApprovalAsync(int loanId);
        /// <summary>
        /// Allows you to assign Loan Officer for existing Loan.
        /// </summary>
        /// <param name="loanId"></param>
        /// <param name="loanOfficerId"></param>
        /// <returns></returns>
        Task AssignLoanOfficerAsync(int loanId, int loanOfficerId);
        /// <summary>
        /// Allows you to unassign the Loan Officer.
        /// </summary>
        /// <param name="loanId"></param>
        /// <returns></returns>
        Task UnAssignLoanOfficerAsync(int loanId);
        Task RejectLoanApplicationAsync(int loanId, DateTime rejectedOnDate);
        Task WithdrawnByApplicant(int loanId, string note, DateTime withdrawnOnDate);
        Task DisburseLoanAsync(int loanId, DateTime actualDisbursementDate,
            decimal? transactionAmount = null, decimal? fixedEmiAmount = null);
        Task DisburseLoanToSavingsAccountAsync(int loanId, int actualDisbursementDate,
            decimal? transactionAmount = null, decimal? fixedEmiAmount = null);
        Task UndoLoanDisbursalAsync(int loanId, string note);
        /// <summary>
        /// Allows you to Submit the GLIM Application. GLIM Application should be in Pending state.
        /// </summary>
        /// <returns></returns>
        Task SubmitGlimApplicationAsync();
        /// <summary>
        /// Allows you to Approve the GLIM Application.
        /// </summary>
        /// <returns></returns>
        Task ApproveGlimApplicationAsync();
        /// <summary>
        /// Allows you to undoApprove the GLIM Application. GLIM application should be in Approved state.
        /// </summary>
        /// <returns></returns>
        Task UndoGlimApplicationApprovalAsync();
        /// <summary>
        /// Allows you to reject the GLIM Application. GLIM application should be in pending state
        /// </summary>
        /// <returns></returns>
        Task RejectGlimApplicationAsync();
        /// <summary>
        /// Allows you to disburse the GLIM Application, GLIM Account should be in Approved state.
        /// </summary>
        /// <returns></returns>
        Task DisburseGlimApplicationAsync();
        /// <summary>
        /// Allows you to undoDisburse the GLIM Application. GLIM application should be in Disburse state.
        /// </summary>
        /// <returns></returns>
        Task UndoDisburseGlimApplicationAsync(int loanId, string? note);
        /// <summary>
        /// Allows you to repayment the GLIM Application, GLIM Application should be in Disburse state.
        /// </summary>
        /// <param name="loanId"></param>
        /// <returns></returns>
        Task RepaymentGlimApplicationAsync(int loanId);
        /// <summary>
        /// Note: Only loans in "Submitted and awaiting approval" status can be deleted.
        /// </summary>
        /// <param name="loanId"></param>
        /// <returns></returns>
        Task DeleteLoanApplicationAsync(int loanId);

        /// <summary>
        /// Capabilities include loan repayment's, interest waivers and the ability
        /// to 'adjust' an existing transaction. An 'adjustment' of a transaction is
        /// really a 'reversal' of existing transaction followed by creation of a
        /// new transaction with the provided details.
        /// </summary>
        public interface ITransactions
        {
            public Task MakeRepaymentAsync();
            public Task MakeRefundOfActiveLoanByCashAsync();
            public Task ForclosureOfAnActiveLoan();
            public Task WaiveInterestAsync();
            public Task WriteOffLoanAsync();
            /// <summary>
            /// This API allows collecting recovery payments for written-off loans
            /// </summary>
            /// <returns></returns>
            public Task MakeRecoveryPaymentAsync();
            public Task UndoLoanWriteOffTransactionAsync();
            /// <summary>
            /// This Api retrieves pre closure details of loan
            /// </summary>
            /// <returns></returns>
            public Task PreCloseTemplateAsync();
            public Task GetTransactionDetails(int transactionId);
            /// <summary>
            /// Note: there is no need to specify command={transactionType} parameter.
            /// </summary>
            /// <param name="transactionId"></param>
            /// <returns></returns>
            public Task AdjustTransaction(int transactionId);

        }
    }
}

