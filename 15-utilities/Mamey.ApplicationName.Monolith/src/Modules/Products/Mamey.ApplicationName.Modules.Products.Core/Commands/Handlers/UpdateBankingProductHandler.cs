using System.Threading;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Products.Core.Events;
using Mamey.ApplicationName.Modules.Products.Core.Exceptions;
using Mamey.ApplicationName.Modules.Products.Core.Mappings;
using Mamey.ApplicationName.Modules.Products.Core.Services;
using Mamey.CQRS.Commands;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.ApplicationName.Modules.Products.Core.Commands.Handlers;

internal sealed class UpdateBankingProductHandler : ICommandHandler<UpdateBankingProduct>
{
    private readonly IBankingProductService _bankingProductService;
    private readonly IMessageBroker _messageBroker;

    public UpdateBankingProductHandler(IBankingProductService bankingProductService, IMessageBroker messageBroker)
    {
        _bankingProductService = bankingProductService;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(UpdateBankingProduct command, CancellationToken cancellationToken = default)
    {
        var existingProduct = await _bankingProductService.GetProductByIdAsync(command.Id);
        if (existingProduct == null)
        {
            throw new ProductNotFoundException(command.Id);
        }
        
        await _bankingProductService.UpdateProductAsync(command.AsEntity(existingProduct));
        await _messageBroker.PublishAsync(new BankingProductUpdated(existingProduct.Id));
    }
}