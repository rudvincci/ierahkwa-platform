using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Government.Modules.Tenant.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Tenant.Core.Commands.Handlers;

internal sealed class UpdateTenantHandler : ICommandHandler<UpdateTenant>
{
    private readonly ITenantRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<UpdateTenantHandler> _logger;

    public UpdateTenantHandler(
        ITenantRepository repository,
        IMessageBroker messageBroker,
        ILogger<UpdateTenantHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(UpdateTenant command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating tenant: {TenantId}", command.TenantId);

        var tenant = await _repository.GetAsync(new TenantId(command.TenantId), cancellationToken);
        if (tenant is null)
        {
            throw new InvalidOperationException($"Tenant with ID {command.TenantId} not found");
        }

        tenant.UpdateDisplayName(command.DisplayName);

        await _repository.UpdateAsync(tenant, cancellationToken);

        // Publish integration event
        await _messageBroker.PublishAsync(
            new TenantUpdatedEvent(tenant.Id.Value, tenant.DisplayName),
            cancellationToken);

        _logger.LogInformation("Tenant updated: {TenantId}", command.TenantId);
    }
}
