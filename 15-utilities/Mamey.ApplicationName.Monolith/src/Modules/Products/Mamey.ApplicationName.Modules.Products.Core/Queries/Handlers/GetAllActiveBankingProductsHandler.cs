using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Products.Core.DTO;
using Mamey.ApplicationName.Modules.Products.Core.Services;
using Mamey.ApplicationName.Modules.Products.Core.Mappings;
using Mamey.CQRS.Queries;

namespace Mamey.ApplicationName.Modules.Products.Core.Queries.Handlers;

internal sealed class GetAllActiveBankingProductsHandler : IQueryHandler<GetAllActiveBankingProducts, IEnumerable<BankingProductDto>>
{
    private readonly IBankingProductService _productService;

    public GetAllActiveBankingProductsHandler(IBankingProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<BankingProductDto>> HandleAsync(GetAllActiveBankingProducts query,
        CancellationToken cancellationToken = default)
    {
        var dtos = (await _productService.GetActiveProductsAsync())?.Select(c => c.AsDto());
        return dtos;
    }
}
internal sealed class GetAllBankingProductsHandler : IQueryHandler<GetAllBankingProducts, IEnumerable<BankingProductDto>>
{
    private readonly IBankingProductService _productService;

    public GetAllBankingProductsHandler(IBankingProductService productService)
    {
        _productService = productService;
    }

    public async Task<IEnumerable<BankingProductDto>> HandleAsync(GetAllBankingProducts query,
        CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetActiveProductsAsync();
        var dtos = products?.Select(c => c.AsDto());
        return dtos;
    }
}