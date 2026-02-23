using Mamey.Mifos.Entities;

namespace Mamey.Microservice.Abstractions.Mifos.Commands.Loans
{
    public class SubmitLoanApplication
    {
        public SubmitLoanApplication()
        {
        }

        #region Mandatory Fields

        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public decimal Principal { get; set; }
        public int LoanTermFrequency { get; set; }
        public FrequencyType LoanTermFrequencyType { get; set; }
        public LoanType LoanType { get; set; }
        public int NumberOfRepayments { get; set; }
        public int RepaymentEvery { get; set; }
        public FrequencyType RepaymentFrequencyType { get; set; }
        public int InterestRatePeriod { get; set; }
        public AmortizationType AmortizationType { get; set; }
        public InterestType InterestType { get; set; }
        public InterestCalculationPeriodType InterestCalculationPeriodType { get; set; }
        public int TransactionProcessingStrategyId { get; set; }
        public DateTime ExpectedDisbursementDate { get; set; }
        public DateTime SubmittedOnDate { get; set; }
        #endregion

        // Mandatory if interest recalculation is enabled for product and
        // rest frequency not same as repayment period.
        public DateTime? RecalculationRestFrequencyDate { get; set; }

        /// <summary>
        /// Mandatory if interest recalculation with interest/fee compounding is
        /// enabled for product and compounding frequency not same as repayment period.
        /// </summary>
        public DateTime? RecalculationCompoundingFrequencyDate { get; set; }

        public IEnumerable<LoanDataTable> LoanDataTables { get; set; }
    }
}

