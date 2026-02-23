using System.Collections.Generic;
using Mamey.ApplicationName.Modules.Products.Core.DTO;
using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Products.Core.Queries;

public record GetProductsByBenefitType(string BenefitType) : IQuery<IEnumerable<BankingProductDto>>;