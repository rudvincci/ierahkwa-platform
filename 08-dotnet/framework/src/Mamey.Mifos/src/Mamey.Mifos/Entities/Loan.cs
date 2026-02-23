namespace Mamey.Mifos.Entities;

public class Loan
{
    public Loan()
    {
    }

    /// <summary>
    /// The account no. associated with this loan. Is auto generated if not provided at loan application creation time.
    /// </summary>
    public string AccountNo { get; set; }
    /// <summary>
    /// A place to put an external reference for this loan e.g. The ID another system uses.
    /// If provided, it must be unique.
    /// </summary>
    public string ExternalId { get; set; }

    /// <summary>
    /// Optional: For associating a loan with a given fund.
    /// </summary>
    public string FundId { get; set; }

    /// <summary>
    /// Optional: For associating a loan with a given staff member who is a loan officer.
    /// </summary>
    public string LoanOfficerId { get; set; }

    /// <summary>
    /// Optional: For marking a loan with a given loan purpose option.
    /// Loan purposes are configurable and can be setup by system admin through code/code values screens.
    /// </summary>
    public string LoanPurposeId { get; set; }

    /// <summary>
    /// The loan amount to be disbursed to through loan.
    /// </summary>
    public decimal Principal { get; set; }

    /// <summary>
    /// The length of loan term.
    /// </summary>
    public int LoanTermFrequency { get; set; }
    /// <summary>
    /// The loan term period to use.
    /// </summary>
    public FrequencyType LoanTermFrequencyType { get; set; }
    /// <summary>
    /// Number of installments to repay.
    /// </summary>
    public int NumberOfRepayments { get; set; }
    /// <summary>
    /// Frequency of repayment.
    /// </summary>
    public int RepaymentEvery { get; set; }
    /// <summary>
    /// Frequency Type of repayment.
    /// </summary>
    public FrequencyType RepaymentFrequencyType { get; set; }
    /// <summary>
    /// Interest rate.
    /// </summary>
    public decimal InterestRatePerPeriod { get; set; }
    /// <summary>
    /// Interest rate frequency type
    /// </summary>
    public FrequencyType InterestRateFrequencyType { get; set; }
    /// <summary>
    /// Optional: Integer - represents the number of repayment periods that
    /// grace should apply to the principal component of a repayment period.
    /// </summary>
    public int? GraceOnPrincipalPayment { get; set; }
    /// <summary>
    /// Optional: Integer - represents the number of repayment periods that
    /// grace should apply to the interest component of a repayment period.
    /// Interest is still calculated but offset to later repayment periods.
    /// </summary>
    public int? GraceOnInterestPayment { get; set; }
    /// <summary>
    /// Optional: Integer - represents the number of repayment periods that
    /// should be interest-free.
    /// </summary>
    public int? GraceOnInterestCharged { get; set; }

    /// <summary>
    /// Optional: Integer - Used in Arrears calculation to only take into account loans that are more than graceOnArrearsAgeing days overdue.
    /// </summary>
    public int? GraceOnArrearsAgeing { get; set; }
    /// <summary>
    /// Optional: Date - The date from with interest is to start being charged.
    /// </summary>
    public DateTime? InterestChargedFromDate { get; set; }
    /// <summary>
    /// The proposed disbursement date of the loan so a proposed repayment schedule can be provided.
    /// </summary>
    public DateTime ExpectedDisbursementDate { get; set; }
    /// <summary>
    /// The date the loan application was submitted by applicant.
    /// </summary>
    public DateTime SubmittedOnDate { get; set; }
    /// <summary>
    /// The Savings Account id for linking with loan account for payments.
    /// </summary>
    public string LinkAccountId { get; set; }
    public AmortizationType AmortizationType { get; set; }

    public decimal InterestType { get; set; }
    public InterestCalculationPeriodType InterestCalculationPeriodType { get; set; }

    /// <summary>
    /// This value will be supported along with interestCalculationPeriodType
    /// as Same as repayment period to calculate interest for partial periods.
    /// </summary>
    /// <example>Interest charged from is 5th of April , Principal is 10000
    /// and interest is 1% per month then the interest will be (10000 * 1%)* (25/30) ,
    /// it calculates for the month first then calculates exact periods between
    /// start date and end date(can be a decimal)</example>
    public bool AllowPartialPeriodInterestCalcualtion { get; set; }

    /// <summary>
    /// The amount that can be 'waived' at end of all loan payments because it is too small to worry about.
    /// This is also the tolerance amount assessed when determining if a loan is in arrears.
    /// </summary>
    public decimal InArrearsTolerance { get; set; }


    public TransactionProcessingStrategy TransactionProcessingStrategy { get; set; }
    /// <summary>
    /// Specifies rest frequency start date for interest recalculation. This date must be before or equal to disbursement date
    /// </summary>
    public DateTime RecalculationRestFrequencyDate { get; set; }
    /// <summary>
    /// Specifies compounding frequency start date for interest recalculation.
    /// This date must be equal to disbursement date
    /// </summary>
    public DateTime RecalculationCompoundingFrequencyDate => ExpectedDisbursementDate;

}
