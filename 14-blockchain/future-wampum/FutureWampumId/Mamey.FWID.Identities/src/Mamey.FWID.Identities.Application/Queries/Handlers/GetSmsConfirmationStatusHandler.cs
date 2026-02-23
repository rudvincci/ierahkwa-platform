using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

internal sealed class GetSmsConfirmationStatusHandler : IQueryHandler<GetSmsConfirmationStatus, bool>
{
    private readonly ISmsConfirmationService _smsConfirmationService;

    public GetSmsConfirmationStatusHandler(ISmsConfirmationService smsConfirmationService)
    {
        _smsConfirmationService = smsConfirmationService ?? throw new ArgumentNullException(nameof(smsConfirmationService));
    }

    public async Task<bool> HandleAsync(GetSmsConfirmationStatus query, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(query.IdentityId);
        return await _smsConfirmationService.IsPhoneConfirmedAsync(identityId, cancellationToken);
    }
}

