using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetPendingEmailConfirmationsHandler : IQueryHandler<GetPendingEmailConfirmations, IEnumerable<EmailConfirmationDto>>
{
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;

    public GetPendingEmailConfirmationsHandler(IEmailConfirmationRepository emailConfirmationRepository)
    {
        _emailConfirmationRepository = emailConfirmationRepository;
    }

    public async Task<IEnumerable<EmailConfirmationDto>> HandleAsync(GetPendingEmailConfirmations query, CancellationToken cancellationToken = default)
    {
        var emailConfirmations = await _emailConfirmationRepository.GetPendingAsync(cancellationToken);
        return emailConfirmations.Select(MapToEmailConfirmationDto);
    }

    private static EmailConfirmationDto MapToEmailConfirmationDto(EmailConfirmation emailConfirmation)
    {
        return new EmailConfirmationDto(
            emailConfirmation.Id,
            emailConfirmation.UserId,
            emailConfirmation.Email,
            emailConfirmation.ConfirmationCode,
            emailConfirmation.ExpiresAt,
            emailConfirmation.IpAddress,
            emailConfirmation.UserAgent,
            emailConfirmation.Status.ToString(),
            emailConfirmation.CreatedAt,
            emailConfirmation.ConfirmedAt
        );
    }
}
