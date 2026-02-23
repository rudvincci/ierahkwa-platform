using Mamey.Exceptions;

namespace Mamey.ApplicationName.Modules.Identity.Core.Exceptions
{
    public class ApplicationUserNotFoundException : MameyException
    {
        public Guid Id { get; }

        public ApplicationUserNotFoundException(Guid id) : base($"ApplicationUser with id '{id} was not found.'")
            => Id = id;
    }
}