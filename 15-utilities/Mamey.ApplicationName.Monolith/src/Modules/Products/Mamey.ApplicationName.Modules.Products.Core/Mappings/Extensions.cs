using System;
using System.Linq;
using Mamey.Bank.Modules.BankingProducts.Core.Entities;
using Mamey.ApplicationName.Modules.Products.Core.Commands;
using Mamey.ApplicationName.Modules.Products.Core.DTO;
using Mamey.ApplicationName.Modules.Products.Core.Entities;
using Mamey;

using AccountCategory = Mamey.Bank.Shared.Types.AccountCategory;


namespace Mamey.ApplicationName.Modules.Products.Core.Mappings
{
    internal static class Extensions
    {
        // Map Product entity to BankingProductDTO
        internal static BankingProductDto AsDto(this Product product)
        {
            return new BankingProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Currency = product.Currency,
                ProductType = product.ProductType.ToString(),
                AccountCategory = product.AccountCategory.ToString(),
                InterestRate = product.InterestRate?.AsInterestRateDto(),
                Status = product.Status,
                CreatedDate = product.CreatedDate,
                ModifiedDate = product.ModifiedDate,
                EligibilityCriteria = product.EligibilityCriteria.AsEligibilityCriteriaDto(),
                ApplicableTaxes = product.ApplicableTaxes.Select(c=> c.AsApplicableTaxeDto()),
                RequiredDocuments = product.RequiredDocuments.Select(c=> c.AsRequiredDocumentDto()),
                Benefits = product.Benefits.Select(c=> c.AsBenefitDto()),
                Fees = product.Fees.Select(c=> c.AsFeeDto()),
                Limits = product.Limits.AsLimitsDto(),
                TermsAndConditions = product.TermsAndConditions,
            };
        }

        internal static InterestRateDto AsInterestRateDto(this InterestRate rate)
        {
            return new()
            {
                Id = rate.Id,
                Rate = rate.Rate,
                CompoundingFrequency = rate.CompoundingFrequency,
                Type = rate.Type.ToString(),
            };
        }

        // Map Product entity to BankingProductSummaryDTO
        internal static BankingProductSummaryDto AsSummaryDto(this Product product)
        {
            return new BankingProductSummaryDto
            {
                Id = product.Id,
                Name = product.Name,
                ProductType = product.ProductType.ToString(),
                AccountCategory = product.AccountCategory.ToString(),
                Status = product.Status.ToString(),
            };
        }

        // Map CreateBankingProductDTO to Product entity
        internal static Product AsEntity(this CreateBankingProduct dto)
        {
            return new Product
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                // Currencies = dto.Currency,
                ProductType = Enum.Parse<AccountType>(dto.ProductType, true),
                AccountCategory = Enum.Parse<AccountCategory>(dto.AccountCategory, true),
                InterestRate = new InterestRate
                {
                    Rate = dto.InterestRate,
                    Type = Enum.Parse<InterestRateType>(dto.InterestRateType, true)
                },
                Status = dto.Status.ToEnum<ProductStatus>(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }

        // Map UpdateBankingProductDTO to Product entity
        internal static Product AsEntity(this UpdateBankingProduct dto, Product existingProduct)
        {
            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            // existingProduct.Currencies = dto.Currency.();
            existingProduct.ProductType = dto.ProductType.ToEnum<AccountType>();
            existingProduct.AccountCategory = dto.ProductType.ToEnum<AccountCategory>();
            existingProduct.InterestRate.Rate = dto.InterestRate;
            existingProduct.InterestRate.Type = dto.ProductType.ToEnum<InterestRateType>();
            existingProduct.Status = dto.Status.ToEnum<ProductStatus>();
            existingProduct.ModifiedDate = DateTime.UtcNow;

            return existingProduct;
        }

        internal static EligibilityCriteriaDto AsEligibilityCriteriaDto(this EligibilityCriteria criteria)
        {
            return new()
            {
                Id = criteria.Id,
                MinimumIncome = criteria.MinimumIncome,
                OtherCriteria = criteria.OtherCriteria,
                // Geography = criteria.Geographies,
                MaxAge = criteria.MaxAge,
                MinAge = criteria.MinAge,
            };
        }

        internal static ApplicableTaxDto AsApplicableTaxeDto(this ApplicableTax applicableTax)
        {
            return new()
            {
                Id = applicableTax.Id,
                TaxRate = applicableTax.TaxRate,
                TaxName = applicableTax.TaxName,
            };
        }

        internal static RequiredDocumentDto AsRequiredDocumentDto(this RequiredDocument requiredDocument)
        {
            return new RequiredDocumentDto
            {
                Id = requiredDocument.Id,
                DocumentName = requiredDocument.DocumentName,
            };
        }

        internal static BenefitDto AsBenefitDto(this Benefit benefit)
        {
            return new()
            {
                Id = benefit.Id,
                Description = benefit.Description,
                BenefitType = benefit.BenefitType,
            };
        }

        internal static FeeDto AsFeeDto(this Fee fee)
        {
            return new()
            {
                Id = fee.Id,
                Amount = fee.Amount,
                FeeType = fee.FeeType.ToString(),
                Frequency = fee.Frequency.ToString(),

            };
        }
        internal static Fee AsFeeEntity(this FeeDto fee)
        {
            return new()
            {
                Id = fee.Id,
                Amount = fee.Amount,
                FeeType = fee.FeeType.ToEnum<FeeType>(),
                Frequency = fee.Frequency.ToEnum<FeeFrequency>(),

            };
        }

        internal static LimitsDto AsLimitsDto(this Limits limits)
        {
            return new()
            {
                Id = limits.Id,
                MaximumBalance = limits.MaximumBalance,
                MinimumBalance = limits.MinimumBalance,
                DailyTransactionLimit = limits.DailyTransactionLimit,
                WithdrawalLimit = limits.WithdrawalLimit,
            };
        }
    }
}