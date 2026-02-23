using System;
using Mamey.ApplicationName.Modules.Products.Core.DTO;
using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Products.Core.Queries;

public record GetBakingProductById(Guid bankingProductId) : IQuery<BankingProductDto>;