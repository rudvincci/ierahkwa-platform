using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Queries.Handlers;

internal sealed class GetEmailConfirmationStatusHandler : IQueryHandler<GetEmailConfirmationStatus, bool>
{
    private readonly IEmailConfirmationService _emailConfirmationService;

    public GetEmailConfirmationStatusHandler(IEmailConfirmationService emailConfirmationService)
    {
        _emailConfirmationService = emailConfirmationService ?? throw new ArgumentNullException(nameof(emailConfirmationService));
    }

    public async Task<bool> HandleAsync(GetEmailConfirmationStatus query, CancellationToken cancellationToken = default)
    {
        var identityId = new IdentityId(query.IdentityId);
        return await _emailConfirmationService.IsEmailConfirmedAsync(identityId, cancellationToken);
    }
}

