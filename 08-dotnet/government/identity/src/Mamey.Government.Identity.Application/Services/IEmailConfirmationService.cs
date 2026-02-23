using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Services;

internal interface IEmailConfirmationService
{
    #region Email Confirmation Operations
    Task<string> CreateEmailConfirmationAsync(CreateEmailConfirmation command, CancellationToken cancellationToken = default);
    Task ConfirmEmailAsync(ConfirmEmail command, CancellationToken cancellationToken = default);
    Task ResendEmailConfirmationAsync(ResendEmailConfirmation command, CancellationToken cancellationToken = default);
    #endregion

    #region Email Confirmation Queries
    Task<EmailConfirmationDto?> GetEmailConfirmationAsync(EmailConfirmationId id, CancellationToken cancellationToken = default);
    Task<EmailConfirmationDto?> GetEmailConfirmationByCodeAsync(string confirmationCode, CancellationToken cancellationToken = default);
    Task<EmailConfirmationDto?> GetEmailConfirmationByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    #endregion

    #region Email Confirmation Management
    Task<bool> IsEmailConfirmationValidAsync(string confirmationCode, CancellationToken cancellationToken = default);
    Task CleanupExpiredEmailConfirmationsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<EmailConfirmationDto>> GetPendingEmailConfirmationsAsync(CancellationToken cancellationToken = default);
    #endregion

    #region Email Confirmation Statistics
    Task<EmailConfirmationStatisticsDto> GetEmailConfirmationStatisticsAsync(CancellationToken cancellationToken = default);
    #endregion
}
