using Mamey.Mifos.Entities;
using Mamey.Mifos.Results;

namespace Mamey.Mifos.Services
{
    public sealed class MifosLoansService : IMifosLoansService
    {
        public Task ApproveGlimApplicationAsync()
        {
            throw new NotImplementedException();
        }

        public Task ApproveLoanApplicationAsync(int loanId, DateTime approvedOnDate, decimal? approvedLoanAmount = null, DateTime? expectedDisbursementDate = null)
        {
            throw new NotImplementedException();
        }

        public Task AssignLoanOfficerAsync(int loanId, int loanOfficerId)
        {
            throw new NotImplementedException();
        }

        public Task<LoanRepaymentScheduleResult> CalculateLoanRepaymentScheduleAsync()
        {
            //            {
            //                "dateFormat": "dd MMMM yyyy",
            //    "locale": "en_GB",
            //    "productId": 1,
            //    "principal": "100,000.00",
            //    "loanTermFrequency": 12,
            //    "loanTermFrequencyType": 2,
            //    "numberOfRepayments": 12,
            //    "repaymentEvery": 1,
            //    "repaymentFrequencyType": 2,
            //    "interestRatePerPeriod": 2,
            //    "amortizationType": 1,
            //    "interestType": 0,
            //    "interestCalculationPeriodType": 1,
            //    "expectedDisbursementDate": "20 September 2011",
            //    "transactionProcessingStrategyId": 2
            //}
            throw new NotImplementedException();
        }

        public Task DeleteLoanApplicationAsync(int loanId)
        {
            throw new NotImplementedException();
        }

        public Task DisburseGlimApplicationAsync()
        {
            throw new NotImplementedException();
        }

        public Task DisburseLoanAsync(int loanId, DateTime actualDisbursementDate, decimal? transactionAmount = null, decimal? fixedEmiAmount = null)
        {
            throw new NotImplementedException();
        }

        public Task DisburseLoanToSavingsAccountAsync(int loanId, int actualDisbursementDate, decimal? transactionAmount = null, decimal? fixedEmiAmount = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Loan>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Loan> GetAsync(int loanId)
        {
            throw new NotImplementedException();
        }

        public Task RejectGlimApplicationAsync()
        {
            throw new NotImplementedException();
        }

        public Task RejectLoanApplicationAsync(int loanId, DateTime rejectedOnDate)
        {
            throw new NotImplementedException();
        }

        public Task RepaymentGlimApplicationAsync(int loanId)
        {
            throw new NotImplementedException();
        }

        public Task SubmitGlimApplicationAsync()
        {
            throw new NotImplementedException();
        }

        public Task SubmitNewLoanApplicationAsync()
        {
            throw new NotImplementedException();
        }

        public Task UnAssignLoanOfficerAsync(int loanId)
        {
            throw new NotImplementedException();
        }

        public Task UndoApprovalAsync(int loanId)
        {
            throw new NotImplementedException();
        }

        public Task UndoDisburseGlimApplicationAsync(int loanId, string? note)
        {
            throw new NotImplementedException();
        }

        public Task UndoGlimApplicationApprovalAsync()
        {
            throw new NotImplementedException();
        }

        public Task UndoLoanDisbursalAsync(int loanId, string note)
        {
            throw new NotImplementedException();
        }

        public Task WithdrawnByApplicant(int loanId, string note, DateTime withdrawnOnDate)
        {
            throw new NotImplementedException();
        }
    }
}

