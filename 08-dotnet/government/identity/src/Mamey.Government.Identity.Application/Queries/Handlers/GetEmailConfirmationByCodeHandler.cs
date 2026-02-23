using Mamey.CQRS.Queries;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Application.Exceptions;
using Mamey.Government.Identity.Contracts.Queries;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;

namespace Mamey.Government.Identity.Application.Queries.Handlers;

internal sealed class GetEmailConfirmationByCodeHandler : IQueryHandler<GetEmailConfirmationByCode, EmailConfirmationDto>
{
    private readonly IEmailConfirmationRepository _emailConfirmationRepository;

    public GetEmailConfirmationByCodeHandler(IEmailConfirmationRepository emailConfirmationRepository)
    {
        _emailConfirmationRepository = emailConfirmationRepository;
    }

    public async Task<EmailConfirmationDto> HandleAsync(GetEmailConfirmationByCode query, CancellationToken cancellationToken = default)
    {
        var emailConfirmation = await _emailConfirmationRepository.GetByConfirmationCodeAsync(query.ConfirmationCode, cancellationToken);
        
        if (emailConfirmation is null)
        {
            throw new EmailConfirmationNotFoundException(query.ConfirmationCode);
        }

        return MapToEmailConfirmationDto(emailConfirmation);
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
