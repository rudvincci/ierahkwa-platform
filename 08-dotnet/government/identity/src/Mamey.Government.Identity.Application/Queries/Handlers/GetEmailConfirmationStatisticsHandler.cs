using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Time;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetEmailConfirmationStatisticsHandler : IQueryHandler<GetEmailConfirmationStatistics, EmailConfirmationStatisticsDto>
{
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;
    private readonly IClock _clock;

    public GetEmailConfirmationStatisticsHandler(IEmailConfirmationRepository emailConfirmationRepository, IClock clock)
    {
        _emailConfirmationRepository = emailConfirmationRepository;
        _clock = clock;
    }

    public async Task<EmailConfirmationStatisticsDto> HandleAsync(GetEmailConfirmationStatistics query, CancellationToken cancellationToken = default)
    {
        var totalConfirmations = await _emailConfirmationRepository.CountAsync(cancellationToken);
        var pendingConfirmations = await _emailConfirmationRepository.CountByStatusAsync(EmailConfirmationStatus.Pending, cancellationToken);
        var confirmedConfirmations = await _emailConfirmationRepository.CountByStatusAsync(EmailConfirmationStatus.Confirmed, cancellationToken);
        var expiredConfirmations = await _emailConfirmationRepository.CountExpiredAsync(cancellationToken);

        return new EmailConfirmationStatisticsDto
        {
            TotalConfirmations = totalConfirmations,
            PendingConfirmations = pendingConfirmations,
            ConfirmedConfirmations = confirmedConfirmations,
            ExpiredConfirmations = expiredConfirmations
        };
    }
}
