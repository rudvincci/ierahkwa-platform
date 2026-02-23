using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Modules.Citizens.Core.Commands.Handlers;

internal sealed class UpdateCitizenHandler : ICommandHandler<UpdateCitizen>
{
    private readonly ICitizenRepository _repository;

    public UpdateCitizenHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateCitizen command, CancellationToken cancellationToken = default)
    {
        var citizen = await _repository.GetAsync(new CitizenId(command.CitizenId), cancellationToken);
        if (citizen is null)
        {
            throw new CitizenNotFoundException(command.CitizenId);
        }

        // Update personal details if names provided
        var firstName = !string.IsNullOrWhiteSpace(command.FirstName)
            ? command.FirstName
            : citizen.FirstName;
        var lastName = !string.IsNullOrWhiteSpace(command.LastName)
            ? command.LastName
            : citizen.LastName;
        
        var citizenName = new Name(firstName, lastName);
        citizen.UpdatePersonalDetails(citizenName, citizen.DateOfBirth);

        // Update contact if email, phone, or address provided
        Email? email = citizen.Email;
        if (command.Email is not null)
        {
            email = string.IsNullOrWhiteSpace(command.Email) ? null : new Email(command.Email);
        }

        Phone? phone = citizen.Phone;
        if (command.Phone is not null)
        {
            phone = string.IsNullOrWhiteSpace(command.Phone) ? null : new Phone("1", command.Phone);
        }

        Address? address = citizen.Address;
        if (command.Street is not null)
        {
            address = string.IsNullOrWhiteSpace(command.Street) ? null : new Address(
                command.Street,
                command.Street,  // street1
                null,            // street2
                null,            // apt
                null,            // building
                command.City ?? string.Empty,
                command.State ?? string.Empty,
                command.Country ?? "USA",
                command.PostalCode,
                null,            // region
                command.Country ?? "US",
                null,            // country code
                false,           // isPrimary
                Address.AddressType.Home);
        }

        citizen.UpdateContact(email, phone, address);

        // Update photo if provided
        if (!string.IsNullOrWhiteSpace(command.PhotoUrl))
        {
            citizen.UpdatePhoto(command.PhotoUrl);
        }

        await _repository.UpdateAsync(citizen, cancellationToken);
    }
}
