using System.Threading;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Products.Core.Events;
using Mamey.ApplicationName.Modules.Products.Core.Exceptions;
using Mamey.ApplicationName.Modules.Products.Core.Mappings;
using Mamey.ApplicationName.Modules.Products.Core.Services;
using Mamey.CQRS.Commands;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Time;

namespace Mamey.ApplicationName.Modules.Products.Core.Commands.Handlers;

internal sealed class CreateBankingProductHandler : ICommandHandler<CreateBankingProduct>
{
    private readonly IBankingProductService _bankingProductService;

    private readonly IMessageBroker _messageBroker;

    public CreateBankingProductHandler(IBankingProductService bankingProductService, IMessageBroker messageBroker)
    {
        _bankingProductService = bankingProductService;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(CreateBankingProduct command, CancellationToken cancellationToken = default)
    {
        var existingProduct = await _bankingProductService.GetProductByNameAsync(command.Name);
        if (existingProduct != null)
        {
            throw new ProductAlreadyExistsException(command.Name);
        }

        existingProduct = await _bankingProductService.GetProductByIdAsync(command.Id);
        if (existingProduct != null)
        {
            throw new ProductAlreadyExistsException(command.Id);
        }

        await _bankingProductService.AddProductAsync(command.AsEntity());
        await _messageBroker.PublishAsync(new BankingProductCreated(existingProduct.Id), cancellationToken);
    }
}