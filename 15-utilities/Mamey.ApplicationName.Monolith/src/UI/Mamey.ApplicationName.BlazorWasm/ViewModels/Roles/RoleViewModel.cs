using System.Collections.ObjectModel;
using Mamey.ApplicationName.BlazorWasm.Models.Roles;
using Mamey.ApplicationName.BlazorWasm.Services.Roles;
using ReactiveUI;

namespace Mamey.ApplicationName.BlazorWasm.ViewModels.Roles
{
    public class RoleViewModel : ReactiveObject
    {
        private readonly RoleService _service;

        public ObservableCollection<Role> Roles { get; }
        public ObservableCollection<User> Users { get; }
        public ObservableCollection<UserActivity> UserActivities { get; }

        public RoleViewModel(RoleService roleService)
        {
            _service = new RoleService();
            Roles = new ObservableCollection<Role>(_service.GetRoles());
            Users = new ObservableCollection<User>(_service.GetUsers());
            UserActivities = new ObservableCollection<UserActivity>(_service.GetActivities());
        }

        public void AddRole(Role newRole)
        {
            Roles.Add(newRole);
            _service.AddRole(newRole);
        }

        public void AssignRoleToUser(User user, string roleName)
        {
            user.AssignedRole = roleName;
            _service.AssignRoleToUser(user, roleName);
            this.RaisePropertyChanged(nameof(Users));
        }

        public void LogActivity(string userName, string activity)
        {
            _service.LogActivity(userName, activity);
            UserActivities.Add(new UserActivity
            {
                UserName = userName,
                Activity = activity,
                Timestamp = System.DateTime.Now
            });
            this.RaisePropertyChanged(nameof(UserActivities));
        }
    }
}