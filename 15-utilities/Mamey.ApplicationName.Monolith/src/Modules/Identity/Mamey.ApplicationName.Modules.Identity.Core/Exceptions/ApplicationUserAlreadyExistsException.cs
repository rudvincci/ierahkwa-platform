using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Identity.Core.Exceptions
{
    public sealed class ApplicationUserAlreadyExistsException : MameyException
    {
        public Guid Id { get; }

        public ApplicationUserAlreadyExistsException(Guid id) : base($"ApplicationUser with id: '{id}' already exists.")
            => Id = id;
    }
}