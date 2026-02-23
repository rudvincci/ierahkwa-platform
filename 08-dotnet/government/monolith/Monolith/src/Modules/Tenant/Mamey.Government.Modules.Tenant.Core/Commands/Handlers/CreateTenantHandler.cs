using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Government.Modules.Tenant.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Tenant.Core.Commands.Handlers;

internal sealed class CreateTenantHandler : ICommandHandler<CreateTenant>
{
    private readonly ITenantRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<CreateTenantHandler> _logger;

    public CreateTenantHandler(
        ITenantRepository repository,
        IMessageBroker messageBroker,
        ILogger<CreateTenantHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(CreateTenant command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating tenant: {DisplayName}", command.DisplayName);

        var tenant = new TenantEntity(
            new TenantId(command.Id),
            command.DisplayName,
            command.Domain);

        await _repository.AddAsync(tenant, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new TenantCreatedEvent(tenant.Id.Value, tenant.DisplayName),
            cancellationToken);

        _logger.LogInformation("Tenant created: {TenantId} - {DisplayName}", 
            command.Id, command.DisplayName);
    }
}
