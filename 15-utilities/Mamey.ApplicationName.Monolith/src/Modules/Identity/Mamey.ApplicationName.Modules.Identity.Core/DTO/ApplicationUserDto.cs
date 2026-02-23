using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Identity.Core.DTO
{
    public class ApplicationUserDto
    {
        public ApplicationUserDto(){}
        public ApplicationUserDto(Guid userId, string email, string userName, string role, string status, Name name, bool emailConfirmed, bool lockoutEnabled)
        {
            this.UserId = userId;
            this.Email = email;
            UserName = userName;
            Role = role;
            Status = status;
            this.Name = name;
            this.EmailConfirmed = emailConfirmed;
            this.LockoutEnabled = lockoutEnabled;
        }
        public Guid UserId { get; }
        public string UserName { get; set; }
        public string Email { get; }
        public string Role { get; set; }
        public Name Name { get; }
        public bool EmailConfirmed { get; }
        public bool LockoutEnabled { get; }
  
        private string Status { get; set; }
    }
}