using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.DTO;

namespace Mamey.Government.Modules.Passports.Core.Queries.Handlers;

internal sealed class ValidatePassportHandler : IQueryHandler<ValidatePassport, PassportValidationResultDto?>
{
    private readonly IPassportRepository _repository;

    public ValidatePassportHandler(IPassportRepository repository)
    {
        _repository = repository;
    }

    public async Task<PassportValidationResultDto?> HandleAsync(ValidatePassport query, CancellationToken cancellationToken = default)
    {
        var passport = await _repository.GetByPassportNumberAsync(new PassportNumber(query.PassportNumber), cancellationToken);
        
        if (passport is null)
        {
            return new PassportValidationResultDto
            {
                IsValid = false,
                PassportNumber = query.PassportNumber,
                Status = "NotFound",
                VerifiedAt = DateTime.UtcNow,
                Message = "Passport not found in the system"
            };
        }

        var isExpired = passport.ExpiryDate < DateTime.UtcNow;
        var isActive = passport.IsActive && !isExpired;

        return new PassportValidationResultDto
        {
            IsValid = isActive,
            PassportNumber = passport.PassportNumber.Value,
            Status = isExpired ? "Expired" : (passport.IsActive ? "Active" : "Revoked"),
            HolderName = null, // Holder details require citizen lookup - not available directly on passport
            ExpiresAt = passport.ExpiryDate,
            VerifiedAt = DateTime.UtcNow,
            Message = isActive ? "Passport is valid" : (isExpired ? "Passport has expired" : "Passport has been revoked")
        };
    }
}
