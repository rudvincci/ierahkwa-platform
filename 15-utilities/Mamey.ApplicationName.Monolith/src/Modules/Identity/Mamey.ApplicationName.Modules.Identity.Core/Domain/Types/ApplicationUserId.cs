using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.Domain.Types
{
    public class ApplicationUserId : TypeId
    {
        public ApplicationUserId(Guid value) : base(value)
        {
        }

        public static implicit operator ApplicationUserId(Guid id) => new(id);
        
        public static implicit operator Guid(ApplicationUserId id) => id.Value;
    }
}