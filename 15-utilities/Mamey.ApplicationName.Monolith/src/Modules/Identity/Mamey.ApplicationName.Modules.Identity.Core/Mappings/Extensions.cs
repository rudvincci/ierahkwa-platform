using Mamey.ApplicationName.Modules.Identity.Core.DTO;
using Mamey.Auth.Identity.Entities;

namespace Mamey.ApplicationName.Modules.Identity.Core.Mappings
{
    internal static class Extensions
    {
        public static ApplicationUserDto AsDto(this ApplicationUser applicationuser)
            => new(applicationuser.Id, applicationuser.Email, applicationuser.UserName, null, (applicationuser.LockoutEnabled? "Active"  :"Locked"), applicationuser.Name, 
                applicationuser.EmailConfirmed, applicationuser.LockoutEnabled);
        
    }
}