using Mamey.Types;
using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.Configuration
{
    public class AuthenticatedUser : ReactiveObject
    {
        private Guid _userId = Guid.Empty;
        private Name _name = new Name();
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _role = string.Empty;
        private bool _emailConfirmed = false;
        private bool _lockoutEnabled = false;
        private Dictionary<string, IEnumerable<string>> _claims = new();

        public UserPreferences UserPreferences { get; set; } = new();

        public Guid UserId
        {
            get => _userId;
            set => this.RaiseAndSetIfChanged(ref _userId, value);
        }

        public Name Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string Email
        {
            get => _email;
            set => this.RaiseAndSetIfChanged(ref _email, value);
        }

        public string Role
        {
            get => _role;
            set => this.RaiseAndSetIfChanged(ref _role, value);
        }
        public bool EmailConfirmed
        {
            get => _emailConfirmed;
            set => this.RaiseAndSetIfChanged(ref _emailConfirmed, value);
        }
        public bool LockoutEnabled
        {
            get => _lockoutEnabled;
            set => this.RaiseAndSetIfChanged(ref _lockoutEnabled, value);
        }

        public Dictionary<string, IEnumerable<string>> Claims
        {
            get => _claims;
            set => this.RaiseAndSetIfChanged(ref _claims, value);
        }
    }
}