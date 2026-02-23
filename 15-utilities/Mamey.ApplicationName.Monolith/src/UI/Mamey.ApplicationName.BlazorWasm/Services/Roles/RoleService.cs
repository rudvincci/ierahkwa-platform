using Mamey.ApplicationName.BlazorWasm.Models.Roles;

namespace Mamey.ApplicationName.BlazorWasm.Services.Roles
{
    public class RoleService
    {
        private readonly List<Role> _roles;
        private readonly List<User> _users;
        private readonly List<UserActivity> _activities;

        public RoleService()
        {
            _roles = GenerateMockRoles();
            _users = GenerateMockUsers();
            _activities = GenerateMockActivities();
        }

        public List<Role> GetRoles() => _roles;

        public List<User> GetUsers() => _users;

        public List<UserActivity> GetActivities() => _activities;

        public void AddRole(Role role)
        {
            _roles.Add(role);
        }

        public void AssignRoleToUser(User user, string roleName)
        {
            user.AssignedRole = roleName;
            _activities.Add(new UserActivity
            {
                UserName = user.Name,
                Activity = $"Assigned role '{roleName}'",
                Timestamp = DateTime.Now
            });
        }

        public void LogActivity(string userName, string activity)
        {
            _activities.Add(new UserActivity
            {
                UserName = userName,
                Activity = activity,
                Timestamp = DateTime.Now
            });
        }

        public List<Role> GenerateMockRoles()
        {
            return new List<Role>
            {
                new Role { Id = Guid.NewGuid(), RoleName = "Admin", Permissions = new List<string> { "View Reports", "Manage Users", "Edit Settings" } },
                new Role { Id = Guid.NewGuid(), RoleName = "Manager", Permissions = new List<string> { "View Reports", "Edit Projects" } },
                new Role { Id = Guid.NewGuid(), RoleName = "Support Agent", Permissions = new List<string> { "View Tickets", "Respond to Tickets" } }
            };
        }

        public List<User> GenerateMockUsers()
        {
            return new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "Alice", Email = "alice@example.com", AssignedRole = "Admin" },
                new User { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@example.com", AssignedRole = "Manager" }
            };
        }

        public List<UserActivity> GenerateMockActivities()
        {
            return new List<UserActivity>
            {
                new UserActivity { UserName = "Alice", Activity = "Edited Settings", Timestamp = DateTime.Now.AddMinutes(-30) },
                new UserActivity { UserName = "Bob", Activity = "Created a Project", Timestamp = DateTime.Now.AddHours(-2) }
            };
        }
    }
}
