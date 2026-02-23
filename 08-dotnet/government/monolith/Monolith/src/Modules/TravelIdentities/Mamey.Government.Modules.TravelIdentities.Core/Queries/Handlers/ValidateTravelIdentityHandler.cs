using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries.Handlers;

internal sealed class ValidateTravelIdentityHandler : IQueryHandler<ValidateTravelIdentity, TravelIdentityValidationResultDto?>
{
    private readonly ITravelIdentityRepository _repository;

    public ValidateTravelIdentityHandler(ITravelIdentityRepository repository)
    {
        _repository = repository;
    }

    public async Task<TravelIdentityValidationResultDto?> HandleAsync(ValidateTravelIdentity query, CancellationToken cancellationToken = default)
    {
        var identity = await _repository.GetByTravelIdentityNumberAsync(
            new TravelIdentityNumber(query.IdNumber), 
            cancellationToken);
        
        if (identity is null)
        {
            return new TravelIdentityValidationResultDto
            {
                IsValid = false,
                IdNumber = query.IdNumber,
                Status = "NotFound",
                VerifiedAt = DateTime.UtcNow,
                Message = "Travel identity not found in the system"
            };
        }

        var isExpired = identity.ExpiryDate < DateTime.UtcNow;
        var isActive = identity.IsActive && !isExpired;

        return new TravelIdentityValidationResultDto
        {
            IsValid = isActive,
            IdNumber = identity.TravelIdentityNumber.Value,
            Status = isExpired ? "Expired" : (identity.IsActive ? "Active" : "Revoked"),
            HolderName = null, // Holder details require citizen lookup - not available directly on travel identity
            ExpiresAt = identity.ExpiryDate,
            VerifiedAt = DateTime.UtcNow,
            Message = isActive ? "Travel identity is valid" : (isExpired ? "Travel identity has expired" : "Travel identity has been revoked")
        };
    }
}
