using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;

[assembly: InternalsVisibleTo("Mamey.Bank.Blazor")]
namespace Mamey.ApplicationName.Modules.Products.Core.DTO;

internal class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public List<string> Images { get; set; }
    public bool IsActive { get; set; }
}
internal class BankingProductDto
{
    public Guid Id { get; set; }
    public string ProductType { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Currency { get; set; }
    public bool IsDeleted { get; set; }
    public int Version { get; set; } = 0;
    public string TermsAndConditions { get; set; }
    public ProductStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public InterestRateDto? InterestRate { get; set; } = null;
    public LimitsDto Limits { get; set; } = new LimitsDto();

    public EligibilityCriteriaDto EligibilityCriteria { get; set; }
    public IEnumerable<FeeDto> Fees { get; set; } = new List<FeeDto>();
    public IEnumerable<BenefitDto> Benefits { get; set; } = new List<BenefitDto>();
    public string AccountCategory { get; set; }
    public IEnumerable<RequiredDocumentDto> RequiredDocuments { get; set; } = new List<RequiredDocumentDto>();
    public IEnumerable<ApplicableTaxDto> ApplicableTaxes { get; set; } = new List<ApplicableTaxDto>();
}

public class LimitsDto
{
    public Guid Id { get; set; }
    public decimal? MinimumBalance { get; set; }
    public decimal? MaximumBalance { get; set; }
    public decimal? DailyTransactionLimit { get; set; }
    public decimal? WithdrawalLimit { get; set; }
}
public class ApplicableTaxDto
{
    /// <summary>
    /// Primary key for the ApplicableTax entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the tax (e.g., VAT, GST).
    /// </summary>
    public string TaxName { get; set; }

    /// <summary>
    /// Tax rate applicable to the banking product.
    /// </summary>
    public decimal TaxRate { get; set; }

}

public class BenefitDto
{
    /// <summary>
    /// Primary key for the Benefit entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Type of the benefit (e.g., Cashback, Points, Insurance).
    /// </summary>
    public string BenefitType { get; set; }

    /// <summary>
    /// Description of the benefit.
    /// </summary>
    public string Description { get; set; }
    
}

public class EligibilityCriteriaDto
{
    /// <summary>
    /// Primary key for the EligibilityCriteria entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Minimum age required.
    /// </summary>
    public int? MinAge { get; set; }

    /// <summary>
    /// Maximum age allowed.
    /// </summary>
    public int? MaxAge { get; set; }

    /// <summary>
    /// Minimum income required.
    /// </summary>
    public decimal? MinimumIncome { get; set; }

    /// <summary>
    /// Allowed geography (e.g., country or region).
    /// </summary>
    public string Geography { get; set; }

    /// <summary>
    /// Other criteria in a descriptive format.
    /// </summary>
    public string OtherCriteria { get; set; }
}

public class FeeDto
{
    public Guid Id { get; set; }
    public string FeeType { get; set; }
    public decimal Amount { get; set; }
    public string Frequency { get; set; }
}

public class InterestRateDto
{
    public Guid Id { get; set; }
    public double Rate { get; set; }
    public string Type { get; set; }
    public string CompoundingFrequency { get; set; }
}

public class RequiredDocumentDto
{
    /// <summary>
    /// Primary key for the RequiredDocument entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the required document (e.g., ID Proof, Address Proof).
    /// </summary>
    public string DocumentName { get; set; }
}