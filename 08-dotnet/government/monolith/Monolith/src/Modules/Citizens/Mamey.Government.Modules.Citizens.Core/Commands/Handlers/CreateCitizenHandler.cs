using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.Citizens.Core.Commands.Handlers;

internal sealed class CreateCitizenHandler : ICommandHandler<CreateCitizen>
{
    private readonly ICitizenRepository _repository;

    public CreateCitizenHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateCitizen command, CancellationToken cancellationToken = default)
    {
        var citizenName = new Name(command.FirstName, command.LastName);
        
        // Parse email if provided
        Email? email = !string.IsNullOrWhiteSpace(command.Email) ? new Email(command.Email) : null;
        
        // Parse phone if provided - use default country code "1"
        Phone? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            phone = new Phone("1", command.Phone);
        }
        
        // Parse address if provided
        Address? address = null;
        if (!string.IsNullOrWhiteSpace(command.Street))
        {
            address = new Address(
                firmName: string.Empty,
                line: command.Street,
                line2: null,
                line3: null,
                urbanization: null,
                city: command.City ?? string.Empty,
                state: command.State ?? string.Empty,
                zip5: command.PostalCode?.Length >= 5 ? command.PostalCode[..5] : "00000",
                zip4: command.PostalCode?.Length > 5 ? command.PostalCode[5..] : null,
                postalCode: command.PostalCode,
                country: command.Country ?? "US",
                province: null);
        }

        var citizen = new Citizen(
            new CitizenId(command.CitizenId),
            new TenantId(command.TenantId),
            citizenName,
            email,
            phone,
            address,
            command.DateOfBirth,
            CitizenshipStatus.Probationary);

        if (!string.IsNullOrWhiteSpace(command.PhotoUrl))
        {
            citizen.UpdatePhoto(command.PhotoUrl);
        }

        await _repository.AddAsync(citizen, cancellationToken);
    }
}
