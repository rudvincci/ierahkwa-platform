using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Government.Modules.Tenant.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Tenant.Core.Commands.Handlers;

internal sealed class ActivateTenantHandler : ICommandHandler<ActivateTenant>
{
    private readonly ITenantRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<ActivateTenantHandler> _logger;

    public ActivateTenantHandler(
        ITenantRepository repository,
        IMessageBroker messageBroker,
        ILogger<ActivateTenantHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(ActivateTenant command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Activating tenant: {TenantId}", command.TenantId);

        var tenant = await _repository.GetAsync(new TenantId(command.TenantId), cancellationToken);
        if (tenant is null)
        {
            throw new InvalidOperationException($"Tenant with ID {command.TenantId} not found");
        }

        tenant.Activate();

        await _repository.UpdateAsync(tenant, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new TenantActivatedEvent(tenant.Id.Value),
            cancellationToken);

        _logger.LogInformation("Tenant activated: {TenantId}", command.TenantId);
    }
}
